# Contract Design

This document outlines the ideas for the version 1.0.0 breaking changes to both the client to mGBA-http calls and the mGBA-http to mGBA Lua script calls.

The improvements need to adhere to the [Design](Design.md) document around how easy it is to use for a novice or even a non-programmer as well as being as close to mGBA as possible.

## HTTP API design changes

### Current

Client to mGBA-http relies on HTTP. Pre-1.0.0 it has the downsides of:

- Inconsistent types
  - The `writex` endpoints use a mix of `int` and `uint`
  - Addresses are as strings
- There are two `boolean` representations as `true` and `1`
- Return types are all `text/plain`
- `memoryDomain` and `regName` should be enums but are strings
- Commas are used for hex byte pairs (`d3,00,00,ea`), memory domain names (`cart2,wram,cart0`), and button names (`A,B,Start`) and the client has to transform them

### Improvement ideas

1. Unify the datatypes propertly `int`/`uint` etc 
1. Have the HTTP side take in and return JSON objects as that's just much more normal for APIs compared to `text/plain` which then needs extra processing
1. Proper enums
1. Use a single boolean representation

#### Text vs JSON

The big choice is whether to move from `text/plain`, extremely simple and matches the mGBA design (both design constraints) to `application/json`. The rest are necessary fixes. It does give a bit more overhead, but any modern language with JSON capabilities is so easily equipped to do this that it's a non-issue. 

It might even *be* an issue to have to deal with raw strings instead of assumed API JSON objects. Does this make sense with keep data transformations at a minimum? Technically JSON and mGBA don't talk, but since the overall goal is simplicy, and JSON is the de facto API format, then it probably does make sense to move slightly away from the mGBA shape (negative) to a regular API shape (much more positive). 

## Socket wire changes

### Current

The pre-1.0.0 socket work has the downsides of:

- Each message is a four field string of `$"{Type},{Value1},{Value2},{Value3}"` even if not all the fields are used
- The separator is a comma, which is also valid input for some calls
- Inconsistent list separators:
  - `;`: button lists
  - `,`: hex bytes and domain names
- Weird bracket data type for just `loadStateBuffer`, `[aa,bb,cc]`
- Byte calls have extra overhead by having commas separating byte pairs
- There is both `<|SUCCESS|>` and `<|ERROR|>` 
- mGBA-http doesn't confirm whether the script is the right version for the wire shape

### Improvement ideas

1. A single separator for lists: `,`
1. A field separator that isn't a comma: `|` or `\n`
1. Stop doing the comma separated, square bracket format for bytes (`[aa,bb,cc]`). Just do raw bytes `aabbcc`
1. Have the status as part of the message `OK|value` and `ERR|message`
1. Escape `,` and `|` with their HTML percent codes
1. Simply end terminator to just `<END>`
1. Remove both `<|SUCCESS|>` and `<|ERROR|>` 
1. Send only as many parameters as the command needs
1. Length-prefix framing instead of a terminator

#### Terminator vs length-prefix framing

Length-prefix framing makes it easier for the consumer to deal with data as the length is up front as they know to loop until the n bytes are read. But it requires the sender to know the size of the message then prepend the length. Programatically it's barely harder than appending a terminator. However, I've opted to continue with a terminator for the sole reason of: It's easier to hand write and eyeball a request with a terminator. 

For example, getting the game title in Bash:

```bash
# terminator
printf 'core.getGameTitle<END>' | nc -w1 localhost 8888

# length-prefix
msg='core.getGameTitle'
len=$(printf '%s' "$msg" | wc -c)
printf '%s:%s' "$len" "$msg" | nc -w1 localhost 8888
```

Or Python:

```python
# terminator
def send_terminated(sock, message):
    sock.sendall((message + "<END>").encode())
    received = b""
    while b"<END>" not in received:
        chunk = sock.recv(8192)
        if not chunk:
            raise ConnectionError("socket closed")
        received += chunk
    return received.split(b"<END>", 1)[0].decode()

MAX_FRAME = 8 * 1024 * 1024

# length-prefix
def send_length_prefixed(sock, message):
    body = message.encode()
    sock.sendall(f"{len(body)}:".encode() + body)

    digits = b""
    while True:
        ch = sock.recv(1)
        if not ch:
            raise ConnectionError("socket closed")
        if ch == b":":
            break
        digits += ch

    expected = int(digits)
    if expected > MAX_FRAME:
        raise ValueError(f"frame too large: {expected}")

    payload = b""
    while len(payload) < expected:
        chunk = sock.recv(expected - len(payload))
        if not chunk:
            raise ConnectionError("socket closed")
        payload += chunk

    return payload.decode()
```

A hard to see problem is that it needs to count bytes and not character length. Running `console.log|café` will end up causing junk in further messages as it's 16 characters, but 17 bytes.

However, if it wasn't for the easier reading and hand writing for a terminator, fixed-length would be best for this case.
lastkeys = nil
server = nil
ST_sockets = {}
nextID = 1

local KEY_NAMES = { "A", "B", "s", "S", "<", ">", "^", "v", "R", "L" }

function ST_stop(id)
	local sock = ST_sockets[id]
	ST_sockets[id] = nil
	sock:close()
end

function ST_format(id, msg, isError)
	local prefix = "Socket " .. id
	if isError then
		prefix = prefix .. " Error: "
	else
		prefix = prefix .. " Received: "
	end
	return prefix .. msg
end

function ST_error(id, err)
	console:error(ST_format(id, err, true))
	ST_stop(id)
end

function ST_received(id)
	console:log("ST_received 1")
	local sock = ST_sockets[id]
	if not sock then return end
	while true do
		local p, err = sock:receive(1024)
		console:log("ST_received 2")
		if p then
			console:log("ST_received 3")
			console:log(ST_format(id, p:match("^(.-)%s*$")))
			console:log(p:match("^(.-)%s*$"))
			
			press_key(3)
			--emu:clearKey(3)

		else
			sock:send("<|ACK|>")
			console:log("ST_received 4")
			if err ~= socket.ERRORS.AGAIN then
				console:log("ST_received 5")
				console:error(ST_format(id, err, true))
				ST_stop(id)
			end
			return
		end
	end
end

function ST_accept()
	local sock, err = server:accept()
	if err then
		console:error(ST_format("Accept", err, true))
		return
	end
	local id = nextID
	nextID = id + 1
	ST_sockets[id] = sock
	sock:add("received", function() ST_received(id) end)
	sock:add("error", function() ST_error(id) end)
	console:log(ST_format(id, "Connected"))
end

-- code via ZachHandley
-- https://discord.com/channels/453962671499509772/979634439237816360/1124075643143995522
local keyQueue = {}
local head = 1
local tail = 1
local totalDuration = 0

-- Function to update key presses
function updateKeys()
    -- Check if the queue is empty
    if head ~= tail then
        -- Get the key press at the head of the queue
        local keyPress = keyQueue[head]
        console:log("currentFrame: " .. emu:currentFrame() .. "startFrame: " .. keyPress.startFrame .. " endFrame: " .. keyPress.endFrame .. " key: " .. keyPress.key)
        -- Check if the current frame is within the key press duration
        if emu:currentFrame() >= keyPress.startFrame and emu:currentFrame() <= keyPress.endFrame and not keyPress.keyPressed then
            -- If the current frame is within the key press duration, press the key
            emu:addKey(keyPress.key)
            keyPress.keyPressed = true
            console:log("Pressed: " .. keyPress.key)
        elseif emu:currentFrame() > keyPress.endFrame then
            -- If the key press duration has ended, release the key and remove it from the queue
            emu:clearKey(keyPress.key)
            head = head + 1
            -- If the queue is now empty, reset the total duration
            if head == tail then
                totalDuration = 0
            end
        end
    end
end

-- Function to add a key press
function pressKey(key, duration)
    -- Calculate the start and end frames for this key press
    local startFrame = emu:currentFrame() + totalDuration
    local endFrame = startFrame + duration + 1
    console:log("pressKey: " .. startFrame .. " - " .. endFrame .. " CF: " .. emu:currentFrame())
    -- Add the key, start frame, and end frame to the queue
    keyQueue[tail] = {key = key, startFrame = startFrame, endFrame = endFrame, keyPressed = false}
    tail = tail + 1
    -- Update the total duration
    totalDuration = totalDuration + duration
end

function press_key(key)
    pressKey(key, 10)
    LastKey = key
end


callbacks:add("frame", updateKeys)


local port = 8888
server = nil
while not server do
	server, err = socket.bind(nil, port)
	if err then
		if err == socket.ERRORS.ADDRESS_IN_USE then
			port = port + 1
		else
			console:error(ST_format("Bind", err, true))
			break
		end
	else
		local ok
		ok, err = server:listen()
		if err then
			server:close()
			console:error(ST_format("Listen", err, true))
		else
			console:log("Socket Server Test: Listening on port " .. port)
			server:add("received", ST_accept)
		end
	end
end

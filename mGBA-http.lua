-- ***********************
-- mGBA-http
-- Lua interface for mGBA-http
-- https://github.com/nikouu/mGBA-http
-- https://github.com/nikouu/mGBA-http/blob/main/docs/FullGuide-lua.md
-- ***********************

-- logLevel values
-- 1 = Debug
-- 2 = Information (default)
-- 3 = Warning
-- 4 = Error
-- 5 = None
local logLevel = 2
local truncateLogs = true
local VERSION <const> = "1.0.0"
local TERMINATION_MARKER <const> = "<|END|>"
local DEFAULT_RETURN <const> = "<|SUCCESS|>";
local ERROR_RETURN <const> = "<|ERROR|>";

-- ***********************
-- Sockets
-- ***********************

local server = nil

-- id -> { id, sock, buffer, sendQueue, sendOffset }
--
--   id         - this connection's key in the table above, carried inside the record so functions
--                that take a connection can log it without also being passed the id.
--   sock       - the mGBA socket this connection reads from and writes to.
--   buffer     - bytes received so far. A read can return half a message, or several at once, so
--                bytes accumulate here until TERMINATION_MARKER shows where a message ends.
--   sendQueue  - FIFO of replies waiting to go out. Only the reply at the head is ever in flight.
--   sendOffset - how many bytes of the head reply the socket has accepted. Resets to 0 on dequeue,
--                so it always refers to whichever reply is at the head.
local connections = {}
local nextSocketId = 1
local port = 8888

function beginSocket()
	local err
	while not server do
		server, err = socket.bind(nil, port)
		if err then
			if err == socket.ERRORS.ADDRESS_IN_USE then
				logError("Port ", port, " is already in use. Close whatever is using it, or change 'port' in this script and mgba-http:Socket:Port in appsettings.json to match.")
				break
			else
				logError(formatSocketMessage("Bind", err))
				break
			end
		else
			local ok
			ok, err = server:listen()
			if err then
				server:close()
				logError(formatSocketMessage("Listen", err))
			else
				logWithOverride("mGBA-http script server " .. VERSION .. " ready. Listening on port " .. port, 4)
				server:add("received", socketAccept)
			end
		end
	end
end

function socketAccept()
	local sock, err = server:accept()
	if err then
		logError(formatSocketMessage("Accept", err))
		return
	end
	local id = nextSocketId
	nextSocketId = id + 1
	connections[id] = { id = id, sock = sock, buffer = "", sendQueue = {}, sendOffset = 0 }
	sock:add("received", function() socketReceived(id) end)
	sock:add("error", function() socketError(id) end)
	logDebug("Socket ", id, " connected")
end

function socketReceived(id)
    local conn = connections[id]
    if not conn then return end
    local sock = conn.sock
    while true do
        local chunk, err = sock:receive(1024)
        if chunk then
            conn.buffer = conn.buffer .. chunk
            while true do
                local marker_start, marker_end = conn.buffer:find(TERMINATION_MARKER, 1, true)
                if not marker_start then break end
                local message = conn.buffer:sub(1, marker_start - 1)
                conn.buffer = conn.buffer:sub(marker_end + 1)
                local trimmedMessage = message:match("^(.-)%s*$")
                logDebug("Socket ", id, " Received: ", trimmedMessage)

                local success, returnValue = pcall(function()
                    return messageRouter(trimmedMessage)
                end)

                if not success then
                    logError("Error executing command: ", tostring(returnValue))
                    sendReply(conn, ERROR_RETURN)
                else
                    sendReply(conn, returnValue)
                end
            end
        elseif err then
            -- seems to go into this SOCKETERRORAGAIN state for each call, but it seems fine.
            if err ~= socket.ERRORS.AGAIN then
                if err == "disconnected" then
                    logDebug("Socket ", id, " disconnected")
                elseif err == socket.ERRORS.UNKNOWN_ERROR then
                    -- for some reason this error sometimes happens instead of disconnected
                    logDebug("Socket ", id, " disconnected*")
                else
                    logError(formatSocketMessage(id, err))
                end
                socketStop(id)
            end
            return
        end
    end
end

-- Script sockets are non-blocking, so send accepts as many bytes as the buffer has room for and
-- returns the index of the last byte it took. The buffer only drains while mGBA's event loop runs,
-- and this function is called from a socket callback that is blocking that loop, so retrying here
-- can't make progress. The unsent remainder stays queued on the connection and continues from the frame callback.
function sendReply(conn, payload)
	table.insert(conn.sendQueue, payload .. TERMINATION_MARKER)
	sendPending(conn)
end

function sendPending(conn)
	while #conn.sendQueue > 0 do
		local message = conn.sendQueue[1]
		local sent, err = conn.sock:send(message, conn.sendOffset + 1)

		if err then
			logError("Socket ", conn.id, " send of ", #message, " bytes failed: ", tostring(err))
			conn.sendQueue = {}
			conn.sendOffset = 0
			return
		end

		if sent < #message then
			-- No progress is possible until the event loop runs again, so stop and wait for the frame.
			conn.sendOffset = sent
			logDebug("Socket ", conn.id, " sent ", sent, " of ", #message, " bytes, resuming next frame")
			return
		end

		table.remove(conn.sendQueue, 1)
		conn.sendOffset = 0
		logDebug("Socket ", conn.id, " sent ", #message, " bytes")
	end
end

-- The frame callback is the only continuation point available for sockets that
-- haven't finished sending
callbacks:add("frame", function()
	for _, conn in pairs(connections) do
		sendPending(conn)
	end
end)

function socketStop(id)
	local conn = connections[id]
	connections[id] = nil
	if conn then
		conn.sock:close()
	end
end

function socketError(id)
	logError(formatSocketMessage(id, "Socket error"))
	socketStop(id)
end

function formatSocketMessage(id, msg)
	return "Socket " .. id .. " Error: " .. (msg and tostring(msg) or "Unknown")
end

-- ***********************
-- Message Router
-- ***********************

local keyValues = {
    ["A"] = 0,
    ["B"] = 1,
    ["Select"] = 2,
    ["Start"] = 3,
    ["Right"] = 4,
    ["Left"] = 5,
    ["Up"] = 6,
    ["Down"] = 7,
    ["R"] = 8,
    ["L"] = 9
}

function messageRouter(rawMessage)
    if rawMessage == nil or rawMessage == "" then
        logError("Received an empty message")
        return ERROR_RETURN
    end

    local messageType, rest = rawMessage:match("^([^,]+),(.*)$")
    
    local messageValue1, messageValue2, messageValue3
    
	-- Changes behaviour if the second arugment is an array
    if rest and rest:sub(1,1) == "[" then
        -- Find matching closing bracket
        local bracketCount = 1
        local endBracket
        for i = 2, #rest do
            if rest:sub(i,i) == "[" then
                bracketCount = bracketCount + 1
            elseif rest:sub(i,i) == "]" then
                bracketCount = bracketCount - 1
                if bracketCount == 0 then
                    endBracket = i
                    break
                end
            end
        end
        
        if endBracket then
            messageValue1 = rest:sub(1, endBracket)
            -- Parse remaining values after the bracketed content
            local remaining = rest:sub(endBracket + 2) -- +2 to skip the comma after closing bracket
            if remaining ~= "" then
                local remainingValues = splitStringToTable(remaining, ",")
                messageValue2 = remainingValues[1]
                messageValue3 = remainingValues[2]
            end
        end
    else
        -- Original comma-based parsing for non-bracketed content
        local parsedInput = splitStringToTable(rawMessage, ",")
        messageType = parsedInput[1]
        messageValue1 = parsedInput[2]
        messageValue2 = parsedInput[3]
        messageValue3 = parsedInput[4]
    end



	local returnValue = DEFAULT_RETURN;

	logInformation("Received: ", rawMessage)

	logDebug("messageRouter:",
		"\n\tRaw message: ", rawMessage,
		"\n\tmessageType: ", messageType or "",
		"\n\tmessageValue1: ", messageValue1 or "",
		"\n\tmessageValue2: ", messageValue2 or "",
		"\n\tmessageValue3: ", messageValue3 or "")

	if messageType == "mgba-http.button.add" then addButton(messageValue1)
	elseif messageType == "mgba-http.button.addMany" then addButtons(messageValue1)
	elseif messageType == "mgba-http.button.clear" then clearButton(messageValue1)
	elseif messageType == "mgba-http.button.clearMany" then clearButtons(messageValue1)
	elseif messageType == "mgba-http.button.get" then returnValue = emu:getKey(keyValues[messageValue1])
	elseif messageType == "mgba-http.button.getAll" then returnValue = getAllActiveButtons()
	elseif messageType == "mgba-http.button.tap" then manageButton(messageValue1)
	elseif messageType == "mgba-http.button.tapMany" then manageButtons(messageValue1)
	elseif messageType == "mgba-http.button.hold" then manageButton(messageValue1, messageValue2)
	elseif messageType == "mgba-http.button.holdMany" then manageButtons(messageValue1, messageValue2)
	elseif messageType == "mgba-http.extension.loadFile" then returnValue = loadFile(messageValue1)
	elseif messageType == "core.addKey" then emu:addKey(tonumber(messageValue1))
	elseif messageType == "core.addKeys" then emu:addKeys(tonumber(messageValue1))
	elseif messageType == "core.autoloadSave" then returnValue = emu:autoloadSave()
	elseif messageType == "core.checksum" then returnValue = computeChecksum()
	elseif messageType == "core.clearKey" then emu:clearKey(tonumber(messageValue1))
	elseif messageType == "core.clearKeys" then emu:clearKeys(tonumber(messageValue1))
	elseif messageType == "core.currentFrame" then returnValue = emu:currentFrame()
	elseif messageType == "core.frameCycles" then returnValue = emu:frameCycles()
	elseif messageType == "core.frequency" then returnValue = emu:frequency()
	elseif messageType == "core.getGameCode" then returnValue = emu:getGameCode()
	elseif messageType == "core.getGameTitle" then returnValue = emu:getGameTitle()
	elseif messageType == "core.getKey" then returnValue = emu:getKey(tonumber(messageValue1))
	elseif messageType == "core.getKeys" then returnValue = emu:getKeys()
	elseif messageType == "core.loadFile" then returnValue = emu:loadFile(messageValue1)
	elseif messageType == "core.loadSaveFile" then returnValue = emu:loadSaveFile(messageValue1, toBoolean(messageValue2))
	elseif messageType == "core.loadStateBuffer" then returnValue = emu:loadStateBuffer(convertByteStringToBinary(messageValue1), tonumber(messageValue2))
	elseif messageType == "core.loadStateFile" then returnValue = emu:loadStateFile(messageValue1, tonumber(messageValue2))
	elseif messageType == "core.loadStateSlot" then returnValue = emu:loadStateSlot(tonumber(messageValue1), tonumber(messageValue2))
	elseif messageType == "core.platform" then returnValue = emu:platform()
	elseif messageType == "core.read16" then returnValue = emu:read16(tonumber(messageValue1))
	elseif messageType == "core.read32" then returnValue = emu:read32(tonumber(messageValue1))
	elseif messageType == "core.read8" then returnValue = emu:read8(tonumber(messageValue1))
	elseif messageType == "core.readRange" then returnValue = convertBinaryToByteString(emu:readRange(tonumber(messageValue1), tonumber(messageValue2)))
	elseif messageType == "core.readRegister" then returnValue = tonumber(emu:readRegister(messageValue1))
	elseif messageType == "core.romSize" then returnValue = emu:romSize()
	elseif messageType == "core.saveStateBuffer" then returnValue = convertBinaryToByteString(emu:saveStateBuffer(tonumber(messageValue1)))
	elseif messageType == "core.saveStateFile" then returnValue = emu:saveStateFile(messageValue1, tonumber(messageValue2))
	elseif messageType == "core.saveStateSlot" then returnValue = emu:saveStateSlot(tonumber(messageValue1), tonumber(messageValue2))
	elseif messageType == "core.screenshot" then emu:screenshot(messageValue1)
	elseif messageType == "core.setKeys" then emu:setKeys(tonumber(messageValue1))
	elseif messageType == "core.step" then emu:step()
	elseif messageType == "core.write16" then returnValue = emu:write16(tonumber(messageValue1), tonumber(messageValue2))
	elseif messageType == "core.write32" then returnValue = emu:write32(tonumber(messageValue1), tonumber(messageValue2))
	elseif messageType == "core.write8" then returnValue = emu:write8(tonumber(messageValue1), tonumber(messageValue2))
	elseif messageType == "core.writeRegister" then returnValue = emu:writeRegister(messageValue1, tonumber(messageValue2))
	elseif messageType == "console.error" then console:error(messageValue1)
	elseif messageType == "console.log" then console:log(messageValue1)
	elseif messageType == "console.warn" then console:warn(messageValue1)
	elseif messageType == "coreAdapter.reset" then emu:reset()
	elseif messageType == "coreAdapter.memory" then returnValue = formatMemoryDomains(emu.memory)
	elseif messageType == "memoryDomain.base" then returnValue = emu.memory[messageValue1]:base()
	elseif messageType == "memoryDomain.bound" then returnValue = emu.memory[messageValue1]:bound()
	elseif messageType == "memoryDomain.name" then returnValue = emu.memory[messageValue1]:name()
	elseif messageType == "memoryDomain.read16" then returnValue = emu.memory[messageValue1]:read16(tonumber(messageValue2))
	elseif messageType == "memoryDomain.read32" then returnValue = emu.memory[messageValue1]:read32(tonumber(messageValue2))
	elseif messageType == "memoryDomain.read8" then returnValue = emu.memory[messageValue1]:read8(tonumber(messageValue2))
	elseif messageType == "memoryDomain.readRange" then returnValue = convertBinaryToByteString(emu.memory[messageValue1]:readRange(tonumber(messageValue2), tonumber(messageValue3)))
	elseif messageType == "memoryDomain.size" then returnValue = emu.memory[messageValue1]:size()
	elseif messageType == "memoryDomain.write16" then returnValue = emu.memory[messageValue1]:write16(tonumber(messageValue2), tonumber(messageValue3))
	elseif messageType == "memoryDomain.write32" then returnValue = emu.memory[messageValue1]:write32(tonumber(messageValue2), tonumber(messageValue3))
	elseif messageType == "memoryDomain.write8" then returnValue = emu.memory[messageValue1]:write8(tonumber(messageValue2), tonumber(messageValue3))
	else
		logError("Unable to route raw message: ", rawMessage)
		returnValue = ERROR_RETURN
	end

	logDebug("Raw return: ", tostring(returnValue), " (", type(returnValue), ")")

	if returnValue == false then
		logWarning("mGBA reported failure for: ", rawMessage)
	end

	returnValue = tostring(returnValue or DEFAULT_RETURN);

	logInformation("Returning: ", returnValue)
	return returnValue;
end

function loadFile(path)
	local success = emu:loadFile(path)
	if success then
		emu:reset()
	end
	return success
end

-- ***********************
-- Button (Convenience abstraction)
-- ***********************

function addButton(keyLetter)
	local key = keyValues[keyLetter];
	emu:addKey(key)
end

function clearButton(keyLetter)
	local key = keyValues[keyLetter];
	emu:clearKey(key)
end

function addButtons(keyLetters)
	local keyLettersArray = splitStringToTable(keyLetters, ";")	
	local keys = {}
	for i, keyLetter in ipairs(keyLettersArray) do
		keys[i] = keyValues[keyLetter]
	end
	local bitmask = toBitmask(keys)
	emu:addKeys(bitmask)
end

function clearButtons(keyLetters)
	local keyLettersArray = splitStringToTable(keyLetters, ";")	
	local keys = {}
	for i, keyLetter in ipairs(keyLettersArray) do
		keys[i] = keyValues[keyLetter]
	end
	local bitmask = toBitmask(keys)
	emu:clearKeys(bitmask)
end

function getAllActiveButtons()
    local currentKeys = emu:getKeys()
    local pressedKeys = {}
    
    for keyLetter, keyValue in pairs(keyValues) do
        if (currentKeys & (1 << keyValue)) ~= 0 then
            table.insert(pressedKeys, keyLetter)
        end
    end
    
    return table.concat(pressedKeys, ",")
end

local keyEventQueue = {}

function manageButton(keyLetter, duration)
	duration = duration or 15
	local key = keyValues[keyLetter]
	local bitmask = toBitmask({key})
	enqueueButtons(bitmask, duration)
end

function manageButtons(keyLetters, duration)
	duration = duration or 15
	local keyLettersArray = splitStringToTable(keyLetters, ";")	
	local keys = {}
	for i, keyLetter in ipairs(keyLettersArray) do
		keys[i] = keyValues[keyLetter]
	end
	local bitmask = toBitmask(keys);
	enqueueButtons(bitmask, duration);
end

function enqueueButtons(keyMask, duration)
	local startFrame = emu:currentFrame()
	local endFrame = startFrame + duration + 1

	table.insert(keyEventQueue, 
	{
		keyMask = keyMask,
		startFrame = startFrame, 
		endFrame = endFrame,
		pressed = false
	});
end

function updateKeys()
	local indexesToRemove = {}

	for index, keyEvent in ipairs(keyEventQueue) do

		if emu:currentFrame() >= keyEvent.startFrame and emu:currentFrame() <= keyEvent.endFrame and not keyEvent.pressed then
			emu:addKeys(keyEvent.keyMask)
			keyEvent.pressed = true
		elseif emu:currentFrame() > keyEvent.endFrame then
			emu:clearKeys(keyEvent.keyMask)
			table.insert(indexesToRemove, index)
		end
	end

	-- Remove in reverse so earlier removals don't shift the later indexes
	for i = #indexesToRemove, 1, -1 do
		table.remove(keyEventQueue, indexesToRemove[i])
	end
end

callbacks:add("frame", updateKeys)

-- ***********************
-- Utility
-- ***********************

function splitStringToTable(inputstr, sep)
    if sep == nil then
        sep = "%s"
    end
    local t={}
    for str in string.gmatch(inputstr, "([^"..sep.."]+)") do
        table.insert(t, str)
    end
    return t
end

function toBoolean(str)
    local bool = false
    if string.lower(str) == "true" then
        bool = true
    end
    return bool
end

function computeChecksum()
	local checksum = 0
	for i, v in ipairs({emu:checksum(C.CHECKSUM.CRC32):byte(1, 4)}) do
		checksum = checksum * 256 + v
	end
	return checksum
end

function toBitmask(keys)
    local mask = 0
    for _, key in ipairs(keys) do	
        mask = mask | (1 << tonumber(key))
    end
    return mask
end

function convertBinaryToByteString(binaryString)
    local bytes = {}
    for i = 1, #binaryString do
        table.insert(bytes, string.format("%02x", binaryString:byte(i)))
    end
    return table.concat(bytes, ",")
end

function convertByteStringToBinary(bracketedBytes)
    local hexString = bracketedBytes:match("%[(.+)%]")
    if not hexString then
        logError("Failed to parse bracketed bytes: ", tostring(bracketedBytes))
        return nil
    end
    
    local bytes = {}
    for hexByte in hexString:gmatch("([^,]+)") do
        local byte = tonumber(hexByte, 16)  -- Parse as hex (base 16)
        if byte then
            table.insert(bytes, string.char(byte))
        else
            logError("Invalid hex byte: ", tostring(hexByte))
            return nil
        end
    end
    return table.concat(bytes)
end

function formatMemoryDomains(domains)
    local names = {}
    for name, _ in pairs(domains) do
        table.insert(names, name)
    end
    return table.concat(names, ",")
end

-- ***********************
-- Logging
-- ***********************

function truncate(text)
    if truncateLogs and #text > 500 then
        return string.sub(text, 1, 497) .. "..."
    end
    return text
end

-- Each part is truncated on its own so one long value cannot crowd out the others.
function formatLogMessage(...)
    local parts = table.pack(...)
    for i = 1, parts.n do
        parts[i] = truncate(tostring(parts[i]))
    end
    return "[" .. os.date("%X") .. "] " .. table.concat(parts, "", 1, parts.n)
end

-- The log functions take the message in parts, so nothing is joined unless it is actually written.
function logDebug(...)
    if logLevel <= 1 then
        console:log(formatLogMessage(...))
    end
end

function logInformation(...)
    if logLevel <= 2 then
        console:log(formatLogMessage(...))
    end
end

function logWarning(...)
    if logLevel <= 3 then
        console:warn(formatLogMessage(...))
    end
end

function logError(...)
    if logLevel <= 4 then
        console:error(formatLogMessage(...))
    end
end

function logWithOverride(message, overrideLogLevel)
    if logLevel <= overrideLogLevel then
        console:log(formatLogMessage(message))
    end
end

-- ***********************
-- Start
-- ***********************

beginSocket()
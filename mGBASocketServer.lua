-- ***********************
-- mGBA-http
-- Version: 0.7.0
-- Lua interface for mGBA-http
-- https://github.com/nikouu/mGBA-http
-- ***********************

-- logLevel values
-- 1 = Debug
-- 2 = Information
-- 3 = Warning
-- 4 = Error
-- 5 = None
local logLevel = 2
local TERMINATION_MARKER = "<|END|>"

-- ***********************
-- Sockets
-- ***********************

local server = nil
local socketList = {}
local nextID = 1
local port = 8888

function beginSocket()
	while not server do
		server, error = socket.bind(nil, port)
		if error then
			if error == socket.ERRORS.ADDRESS_IN_USE then
				port = port + 1
			else
				logError(formatSocketMessage("Bind", error, true))
				break
			end
		else
			local ok
			ok, error = server:listen()
			if error then
				server:close()
				logError(formatSocketMessage("Listen", error, true))
			else
				logInformation("mGBA script server 0.7.0 ready. Listening on port " .. port)
				server:add("received", socketAccept)
			end
		end
	end
end

function socketAccept()
	local sock, error = server:accept()
	if error then
		logError(formatSocketMessage("Accept", error, true))
		return
	end
	local id = nextID
	nextID = id + 1
	socketList[id] = sock
	sock:add("received", function() socketReceived(id) end)
	sock:add("error", function() socketError(id) end)
	logDebug(formatSocketMessage(id, "Connected"))
end

function socketReceived(id)
    local sock = socketList[id]
    if not sock then return end
    sock._buffer = sock._buffer or ""
    while true do
        local chunk, error = sock:receive(1024)
        if chunk then
            sock._buffer = sock._buffer .. chunk
            while true do
                local marker_start, marker_end = sock._buffer:find(TERMINATION_MARKER, 1, true)
                if not marker_start then break end
                local message = sock._buffer:sub(1, marker_start - 1)
                sock._buffer = sock._buffer:sub(marker_end + 1)
                logDebug(formatSocketMessage(id, message:match("^(.-)%s*$")))
                -- Route the message and send back the response with the marker
                local returnValue = messageRouter(message:match("^(.-)%s*$"))
                sock:send(returnValue .. TERMINATION_MARKER)
            end
        elseif error then
            -- seems to go into this SOCKETERRORAGAIN state for each call, but it seems fine.
            if error ~= socket.ERRORS.AGAIN then
                if error == "disconnected" then
                    logDebug(formatSocketMessage(id, error, false))
                elseif error == socket.ERRORS.UNKNOWN_ERROR then
                    -- for some reason this error sometimes comes happens instead of disconnected
                    logDebug(formatSocketMessage(id, "disconnected*", false))
                else
                    logError(formatSocketMessage(id, error, true))
                end
                socketStop(id)
            end
            return
        end
    end
end

function socketStop(id)
	local sock = socketList[id]
	socketList[id] = nil
	sock:close()
end

function socketError(id, error)
	logError(formatSocketMessage(id, error, true))
	socketStop(id)
end

function formatSocketMessage(id, msg, isError)
	local prefix = "Socket " .. id
	if isError then
		prefix = prefix .. " Error: "
	else
		prefix = prefix .. " Received: "
	end
	return prefix .. (msg and tostring(msg) or "Probably exceeding limit")
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
	local parsedInput = splitStringToTable(rawMessage, ",")

	local messageType = parsedInput[1]
	local messageValue1 = parsedInput[2]
	local messageValue2 = parsedInput[3]
	local messageValue3 = parsedInput[4]

	local defaultReturnValue <const> = "<|SUCCESS|>";

	local returnValue = defaultReturnValue;

	logInformation("messageRouter: \n\tRaw message: " .. rawMessage .. "\n\tmessageType: " .. (messageType or "") .. "\n\tmessageValue1: " .. (messageValue1 or "") .. "\n\tmessageValue2: " .. (messageValue2 or "") .. "\n\tmessageValue3: " .. (messageValue3 or ""))

	if messageType == "mgba-http.button.tap" then manageButton(messageValue1)
	elseif messageType == "mgba-http.button.tapmany" then manageButtons(messageValue1)
	elseif messageType == "mgba-http.button.hold" then manageButton(messageValue1, messageValue2)
	elseif messageType == "mgba-http.button.holdmany" then manageButtons(messageValue1, messageValue2)
	elseif messageType == "core.addKey" then addKey(messageValue1)
	elseif messageType == "core.addKeys" then emu:addKeys(tonumber(messageValue1))
	elseif messageType == "core.autoloadSave" then returnValue = emu:autoloadSave()
	elseif messageType == "core.checksum" then returnValue = computeChecksum()
	elseif messageType == "core.clearKey" then clearKey(messageValue1)
	elseif messageType == "core.clearKeys" then emu:clearKeys(tonumber(messageValue1))
	elseif messageType == "core.currentFrame" then returnValue = emu:currentFrame()
	elseif messageType == "core.frameCycles" then returnValue = emu:frameCycles()
	elseif messageType == "core.frequency" then returnValue = emu:frequency()
	elseif messageType == "core.getGameCode" then returnValue = emu:getGameCode()
	elseif messageType == "core.getGameTitle" then returnValue = emu:getGameTitle()
	elseif messageType == "core.getKey" then returnValue = emu:getKey(keyValues[messageValue1])
	elseif messageType == "core.getKeys" then returnValue = emu:getKeys()
	elseif messageType == "core.loadFile" then returnValue = emu:loadFile(messageValue1)
	elseif messageType == "core.loadSaveFile" then returnValue = emu:loadSaveFile(messageValue1, toBoolean(messageValue2))
	elseif messageType == "core.loadStateBuffer" then returnValue = emu:loadStateBuffer(messageValue1, messageValue2)
	elseif messageType == "core.loadStateFile" then returnValue = emu:loadStateFile(messageValue1, tonumber(messageValue2))
	elseif messageType == "core.loadStateSlot" then returnValue = emu:loadStateSlot(tonumber(messageValue1), tonumber(messageValue2))
	elseif messageType == "core.platform" then returnValue = emu:platform()
	elseif messageType == "core.read16" then returnValue = emu:read16(tonumber(messageValue1))
	elseif messageType == "core.read32" then returnValue = emu:read32(tonumber(messageValue1))
	elseif messageType == "core.read8" then returnValue = emu:read8(tonumber(messageValue1))
	elseif messageType == "core.readRange" then returnValue = formatByteString(emu:readRange(tonumber(messageValue1), tonumber(messageValue2)))
	elseif messageType == "core.readRegister" then returnValue = tonumber(emu:readRegister(messageValue1))
	elseif messageType == "core.romSize" then returnValue = emu:romSize()
	elseif messageType == "core.runFrame" then emu:runFrame()
	elseif messageType == "core.saveStateBuffer" then formatByteString(emu:saveStateBuffer(tonumber(messageValue1)))
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
	elseif messageType == "memoryDomain.readRange" then returnValue = formatByteString(emu.memory[messageValue1]:readRange(tonumber(messageValue2), tonumber(messageValue3)))
	elseif messageType == "memoryDomain.size" then returnValue = emu.memory[messageValue1]:size()
	elseif messageType == "memoryDomain.write16" then returnValue = emu.memory[messageValue1]:write16(tonumber(messageValue2), tonumber(messageValue3))
	elseif messageType == "memoryDomain.write32" then returnValue = emu.memory[messageValue1]:write32(tonumber(messageValue2), tonumber(messageValue3))
	elseif messageType == "memoryDomain.write8" then returnValue = emu.memory[messageValue1]:write8(tonumber(messageValue2), tonumber(messageValue3))
	elseif (rawMessage == "<|ACK|>") then logInformation("Connecting.")
	elseif (rawMessage ~= nil or rawMessage ~= '') then logInformation("Unable to route raw message: " .. rawMessage)
	else logInformation(messageType)	
	end
	
	returnValue = tostring(returnValue or defaultReturnValue);

	logInformation("Returning: " .. returnValue)
	return returnValue;
end

-- ***********************
-- Button (Convenience abstraction)
-- ***********************

function addKey(keyLetter)
	local key = keyValues[keyLetter];
	emu:addKey(key)
end

function clearKey(keyLetter)
	local key = keyValues[keyLetter];
	emu:clearKey(key)
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

	for _, i in ipairs(indexesToRemove) do
		table.remove(keyEventQueue, i)
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

function numberStringToHex(string)
	return string.format('%x', tonumber(string, 16))
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

function formatByteString(byteStr)
    local bytes = {}
    for i = 1, #byteStr do
        table.insert(bytes, tostring(byteStr:byte(i)))
    end
    return table.concat(bytes, ",")
end

function formatMemoryDomains(domains)
    local names = {}
    for name, _ in pairs(domains) do
        table.insert(names, name)
    end
    return table.concat(names, ", ")
end

-- ***********************
-- Logging
-- ***********************

function logDebug(message)
    if logLevel <= 1 then
        local timestamp = "[" .. os.date("%X", os.time()) .. "] "
        console:log(timestamp .. message)
    end
end

function logInformation(message)
    if logLevel <= 2 then
        local timestamp = "[" .. os.date("%X", os.time()) .. "] "
        console:log(timestamp .. message)
    end
end

function logWarning(message)
    if logLevel <= 3 then
        local timestamp = "[" .. os.date("%X", os.time()) .. "] "
        console:warn(timestamp .. message)
    end
end

function logError(message)
    if logLevel <= 4 then
        local timestamp = "[" .. os.date("%X", os.time()) .. "] "
        console:error(timestamp .. message)
    end
end

-- ***********************
-- Start
-- ***********************

beginSocket()
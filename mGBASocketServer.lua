-- ***********************
-- mGBA-http
-- Version: 0.1.0
-- Lua interface for mGBA-http
-- https://github.com/nikouu/mGBA-http
-- ***********************

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
				console:error(formatMessage("Bind", error, true))
				break
			end
		else
			local ok
			ok, error = server:listen()
			if error then
				server:close()
				console:error(formatMessage("Listen", error, true))
			else
				console:log("Socket Server: Listening on port " .. port)
				server:add("received", socketAccept)
			end
		end
	end
end

function socketAccept()
	local sock, error = server:accept()
	if error then
		console:error(formatMessage("Accept", error, true))
		return
	end
	local id = nextID
	nextID = id + 1
	socketList[id] = sock
	sock:add("received", function() socketReceived(id) end)
	sock:add("error", function() socketError(id) end)
	console:log(formatMessage(id, "Connected"))
end

function socketReceived(id)
	--console:log("socketReceived 1")
	local sock = socketList[id]
	if not sock then return end
	while true do
		local message, error = sock:receive(1024)
	
		--console:log("socketReceived 2")
		if message then
			--console:log("socketReceived 3")
			--console:log(formatMessage(id, message:match("^(.-)%s*$")))
			--console:log(message:match("^(.-)%s*$"))
			
			-- it seems that the value must be non-empty in order to actually send back?
			-- thus the ACK message default
			local returnValue = messageRouter(message:match("^(.-)%s*$"))
			sock:send(returnValue)
		elseif error then
			-- seems to go into this sOCKETERRORAGAIN state for each call, but it seems fine.
			if error ~= socket.ERRORS.AGAIN then
				console:log("socketReceived 4")
				console:error(formatMessage(id, error, true))
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
	console:error(formatMessage(id, error, true))
	socketStop(id)
end

function formatMessage(id, msg, isError)
	local prefix = "Socket " .. id
	if isError then
		prefix = prefix .. " Error: "
	else
		prefix = prefix .. " Received: "
	end
	return prefix .. msg
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

	local defaultReturnValue <const> = "<|ACK|>";
	
	local returnValue = defaultReturnValue;

	console:log("[" .. os.date("%X", os.time()) .. "] messageRouter: \n\tRaw message:" .. rawMessage .. "\n\tmessageType: " .. (messageType or "") .. "\n\tmessageValue1: " .. (messageValue1 or "") .. "\n\tmessageValue2: " .. (messageValue2 or "") .. "\n\tmessageValue3: " .. (messageValue3 or ""))

	if messageType == "custom.button" then pressKey(messageValue1)
	elseif messageType == "core.addKey" then addKey(messageValue1)
	elseif messageType == "core.addKeys" then emu:addKeys(messageValue1)
	elseif messageType == "core.autoloadSave" then returnValue = emu:autoloadSave()
	elseif messageType == "core.checksum" then returnValue = emu:checksum(C.CHECKSUM.CRC32)
	elseif messageType == "core.clearKey" then clearKey(messageValue1)
	elseif messageType == "core.clearKeys" then emu:clearKeys(messageValue1)
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
	elseif messageType == "core.loadStateSlot" then returnValue = emu:loadStateSlot(messageValue1, messageValue2)
	elseif messageType == "core.platform" then returnValue = emu:platform()
	elseif messageType == "core.read16" then returnValue = emu:read16(tonumber(messageValue1))
	elseif messageType == "core.read32" then returnValue = emu:read32(tonumber(messageValue1))
	elseif messageType == "core.read8" then returnValue = emu:read8(tonumber(messageValue1))	
	elseif messageType == "core.readRange" then returnValue = emu:readRange(tonumber(messageValue1), tonumber(messageValue2))
	elseif messageType == "core.readRegister" then returnValue = emu:readRegister(messageValue1)
	elseif messageType == "core.reset" then emu:reset()
	elseif messageType == "core.romSize" then emu:romSize()
	elseif messageType == "core.runFrame" then emu:runFrame()
	elseif messageType == "core.saveStateBuffer" then emu:saveStateBuffer(messageValue1)
	elseif messageType == "core.saveStateFile" then returnValue = emu:saveStateFile(messageValue1, messageValue2)
	elseif messageType == "core.saveStateSlot" then returnValue = emu:saveStateSlot(messageValue1, messageValue2)
	elseif messageType == "core.screenshot" then emu:screenshot(messageValue1)
	elseif messageType == "core.setKeys" then emu:setKeys(messageValue1)
	elseif messageType == "core.step" then emu:step()
	elseif messageType == "core.write16" then returnValue = emu:write16(messageValue1, messageValue2)
	elseif messageType == "core.write32" then returnValue = emu:write32(messageValue1, messageValue2)
	elseif messageType == "core.write8" then returnValue = emu:write8(tonumber(messageValue1), tonumber(messageValue2))
	elseif messageType == "core.writeRegister" then returnValue = emu:writeRegister(messageValue1, messageValue2)
	elseif messageType == "console.error" then console:error(messageValue1)
	elseif messageType == "console.log" then console:log(messageValue1)
	elseif messageType == "console.warn" then console:warn(messageValue1)
	elseif messageType == "coreAdapter.reset" then CoreAdapter:reset()
	elseif messageType == "memoryDomain.base" then returnValue = emu.memory[messageValue1]:base()
	elseif messageType == "memoryDomain.bound" then returnValue = emu.memory[messageValue1]:bound()
	elseif messageType == "memoryDomain.name" then returnValue = emu.memory[messageValue1]:name()
	elseif messageType == "memoryDomain.read16" then returnValue = emu.memory[messageValue1]:read16(tonumber(messageValue2))
	elseif messageType == "memoryDomain.read32" then returnValue = emu.memory[messageValue1]:read32(tonumber(messageValue2))
	elseif messageType == "memoryDomain.read8" then returnValue = emu.memory[messageValue1]:read8(tonumber(messageValue2))
	elseif messageType == "memoryDomain.readRange" then returnValue = emu.memory[messageValue1]:readRange(tonumber(messageValue2), tonumber(messageValue3))
	elseif messageType == "memoryDomain.size" then returnValue = emu.memory[messageValue1]:size()
	elseif messageType == "memoryDomain.write16" then returnValue = emu.memory[messageValue1]:write16(tonumber(messageValue2), tonumber(messageValue3))
	elseif messageType == "memoryDomain.write32" then returnValue = emu.memory[messageValue1]:write32(tonumber(messageValue2), tonumber(messageValue3))
	elseif messageType == "memoryDomain.write8" then returnValue = emu.memory[messageValue1]:write8(tonumber(messageValue2), tonumber(messageValue3))

	else console:log(rawMessage)
	end

	returnValue = tostring(returnValue or defaultReturnValue);

	console:log("\tReturning: " .. returnValue)
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
        --console:log("currentFrame: " .. emu:currentFrame() .. "startFrame: " .. keyPress.startFrame .. " endFrame: " .. keyPress.endFrame .. " key: " .. keyPress.key)
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
function pressKey(keyLetter, duration)
	duration = duration or 15
	local key = keyValues[keyLetter];
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

-- ***********************
-- Start
-- ***********************

--local read = emu:read8(0x00000000)
--local read2 = emu.memory.vram:base()
-- local read2 = emu.memory["vram"]:read8(0x00000000)
--local read2= emu.memory["bios"]:name()
--local read2 = emu.memory["bios"]:read16(tonumber("0x7DE"))
--read2 = emu:read16(tonumber("0x7DE"))
--console:log("" .. read2)

--console:log(emu:memory.currentFrame())

beginSocket()
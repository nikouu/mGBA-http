-- ***********************
-- Sockets
-- ***********************

server = nil
socketList = {}
nextID = 1
local port = 8888

function BeginSocket()
	while not server do
		server, error = socket.bind(nil, port)
		if error then
			if error == socket.ERRORS.ADDRESS_IN_USE then
				port = port + 1
			else
				console:error(FormatMessage("Bind", error, true))
				break
			end
		else
			local ok
			ok, error = server:listen()
			if error then
				server:close()
				console:error(FormatMessage("Listen", error, true))
			else
				console:log("Socket Server Test: Listening on port " .. port)
				server:add("received", SocketAccept)
			end
		end
	end
end

function SocketAccept()
	local sock, error = server:accept()
	if error then
		console:error(FormatMessage("Accept", error, true))
		return
	end
	local id = nextID
	nextID = id + 1
	socketList[id] = sock
	sock:add("received", function() SocketReceived(id) end)
	sock:add("error", function() SocketError(id) end)
	console:log(FormatMessage(id, "Connected"))
end

function SocketReceived(id)
	console:log("SocketReceived 1")
	local sock = socketList[id]
	if not sock then return end
	while true do
		local message, error = sock:receive(1024)
	
		console:log("SocketReceived 2")
		if message then
			console:log("SocketReceived 3")
			console:log(FormatMessage(id, message:match("^(.-)%s*$")))
			console:log(message:match("^(.-)%s*$"))
			
			-- it seems that the value must be non-empty in order to actually send back?
			-- thus the ACK message default
			local returnValue = MessageRouter(message:match("^(.-)%s*$"))
			sock:send(returnValue)
		else
			-- seems to go into this SOCKETERRORAGAIN state for each call, but it seems fine.
			if error ~= socket.ERRORS.AGAIN then
				console:log("SocketReceived 4")
				console:error(FormatMessage(id, error, true))
				SocketStop(id)
			end
			return
		end
	end
end

function SocketStop(id)
	local sock = socketList[id]
	socketList[id] = nil
	sock:close()
end

function SocketError(id, error)
	console:error(FormatMessage(id, error, true))
	SocketStop(id)
end

function FormatMessage(id, msg, isError)
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

function MessageRouter(rawMessage)	
	local parsedInput = splitStringToTable(rawMessage, ",")

	local messageType = parsedInput[1]
	local messageValue1 = parsedInput[2]
	local messageValue2 = parsedInput[3]
	
	local returnValue = "<|ACK|>";

	console:log("MessageRouter: " .. rawMessage .. " messageType: " .. (messageType or "") .. " messageValue1: " .. (messageValue1 or "") .. " messageValue2: " .. (messageValue2 or ""))

	if messageType == "custom.button" then press_key(messageValue1)
	elseif messageType == "core.addKey" then AddKey(messageValue1)
	elseif messageType == "core.addKeys" then emu:addKeys(messageValue1)
	elseif messageType == "core.autoloadSave" then returnValue = emu:autoloadSave()
	elseif messageType == "core.checksum" then returnValue = emu:checksum(C.CHECKSUM.CRC32)
	elseif messageType == "core.clearKey" then emu:clearKey(messageValue1)
	elseif messageType == "core.clearKeys" then emu:clearKeys(messageValue1)
	elseif messageType == "core.currentFrame" then returnValue = emu:currentFrame()
	elseif messageType == "core.frameCycles" then returnValue = emu:frameCycles()
	elseif messageType == "core.frequency" then returnValue = emu:frequency()
	elseif messageType == "core.getGameCode" then returnValue = emu:getGameCode()
	elseif messageType == "core.getGameTitle" then returnValue = emu:getGameTitle()
	elseif messageType == "core.getKey" then returnValue = emu:getKey(messageValue1)
	elseif messageType == "core.getKeys" then returnValue = emu:getKeys()
	elseif messageType == "core.loadSaveFile" then returnValue = emu:loadFile(messageValue1)
	
	--[[	
	TODO: work out how to cleanly deal with multiple input parameters
	elseif messageType == "loadsavefile" then returnValue = emu:loadSaveFile(path, temporary)
	elseif messageType == "loadstatebuffer" then returnValue = emu:loadStateBuffer(buffer, flags)
	elseif messageType == "loadstatefile" then returnValue = emu:loadStateFile(path, flags)
	elseif messageType == "loadstateslot" then returnValue = emu:loadStateSlot(slot, flags)
	elseif messageType == "readrange" then returnValue = emu:readRange(address, length)
	elseif messageType == "savestatefile" then returnValue = emu:saveStateFile(path, flags)
	elseif messageType == "savestateslot" then returnValue = emu:saveStateSlot(path, flags)
	elseif messageType == "write16" then returnValue = emu:write16(address, value)
	elseif messageType == "write32" then returnValue = emu:write32(address, value)
	elseif messageType == "write8" then returnValue = emu:write8(address, value)
	elseif messageType == "writeregister" then returnValue = emu:writeRegister(regName, value)
	--]]

	elseif messageType == "core.platform" then returnValue = emu:platform()
	elseif messageType == "core.read16" then returnValue = emu:read16(tonumber(messageValue1))
	elseif messageType == "core.read32" then returnValue = emu:read32(tonumber(messageValue1))
	elseif messageType == "core.read8" then returnValue = emu:read8(tonumber(messageValue1))	
	elseif messageType == "core.readRegister" then returnValue = emu:readRegister(messageValue1)
	elseif messageType == "core.reset" then emu:reset()
	elseif messageType == "core.romSize" then emu:romSize()
	elseif messageType == "core.runFrame" then emu:runFrame()
	elseif messageType == "core.saveStateBuffer" then emu:saveStateBuffer(messageValue1)
	elseif messageType == "core.screenshot" then emu:screenshot(messageValue1)
	elseif messageType == "core.setKeys" then emu:setKeys(messageValue1)
	elseif messageType == "core.step" then emu:step()

	--elseif messageType == "createbuffer" then console:createBuffer(messageValue1)
	elseif messageType == "console.error" then console:error(messageValue1)
	elseif messageType == "console.log" then console:log(messageValue1)
	elseif messageType == "console.warn" then console:warn(messageValue1)

	-- TODO: check if this is the correct syntax
	elseif messageType == "coreadapter.reset" then CoreAdapter:reset()

	elseif messageType == "memorydomain.base" then returnValue = emu.memory[messageValue1]:base()
	elseif messageType == "memorydomain.bound" then returnValue = emu.memory[messageValue1]:bound()
	elseif messageType == "memorydomain.name" then returnValue = emu.memory[messageValue1]:name()
	elseif messageType == "memorydomain.read16" then returnValue = emu.memory[messageValue1]:read16(tonumber(messageValue2))
	elseif messageType == "memorydomain.read32" then returnValue = emu.memory[messageValue1]:read32(tonumber(messageValue2))
	elseif messageType == "memorydomain.read8" then returnValue = emu.memory[messageValue1]:read8(tonumber(messageValue2))
	elseif messageType == "memorydomain.size" then returnValue = emu.memory[messageValue1]:size()

	else console:log(rawMessage)
	end

	console:log("Returning: " .. returnValue)
	return returnValue;
end

-- ***********************
-- Core
-- ***********************

function AddKey(keyLetter)
	local key = keyValues[keyLetter];
	emu:addKey(key)
end

-- ***********************
-- Button (Convenience abstraction)
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

function press_key(keyLetter)
	local key = keyValues[keyLetter];
    pressKey(key, 15)
    LastKey = key
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

-- ***********************
-- Start
-- ***********************

--local read = emu:read8(0x00000000)
--local read2 = emu.memory.vram:base()
-- local read2 = emu.memory["vram"]:read8(0x00000000)
--local read2= emu.memory["bios"]:name()
local read2 = emu.memory["bios"]:read16(tonumber("0x7DE"))
read2 = emu:read16(tonumber("0x7DE"))
console:log("" .. read2)

--console:log(emu:memory.currentFrame())

BeginSocket()
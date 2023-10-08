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
			
			--press_key(3)
			--emu:clearKey(3)

			MessageRouter(message:match("^(.-)%s*$"))

		else
			sock:send("<|ACK|>")
			console:log("SocketReceived 4")
			if error ~= socket.ERRORS.AGAIN then
				console:log("SocketReceived 5")
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
	local messageType, messageValue = rawMessage:match("([^,]+),([^,]+)")

	if messageType == "button" then press_key(messageValue)
	else console:log(rawMessage)
	end
end

-- ***********************
-- Keys
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
-- Start
-- ***********************

BeginSocket()
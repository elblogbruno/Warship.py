# Socket is confused. Close and remove it.
            client.setsockopt(zmq.LINGER, 0)
            client.close()
            poll.unregister(client)
            retries_left -= 1
            if retries_left == 0:
                print("E: Server seems to be offline, abandoning")
                break
            print("I: Reconnecting and resending (%s)" % request)
            # Create new connection
            client = context.socket(zmq.REQ)
            client.connect(SERVER_ENDPOINT)
            poll.register(client, zmq.POLLIN)
            client.send(request)
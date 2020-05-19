#include <stdio.h>
#include <stdlib.h>
#include <cstdlib>

#include <iostream>
#include <istream>
#include <sstream>
#include <fstream>

#include <io.h>
#include <fcntl.h>
#include <Windows.h>

#include <string>
#include <iterator>
#include <algorithm>
#include <vector>
#include "SDL_net.h"

#ifndef _O_U16TEXT
#define _O_U16TEXT 0x20000
#endif
	
#define PORT 27015
#define TIMEOUT_MS 5000
int maxClientID = 0;

char PACKET_ACK[2] = { '6','5' };
char PACKET_BYE[2] = { '6','6' };
char PACKET_SNAP[2] ={ '8','3' };
char PACKET_HIT[2] = { '7','2' };
char PACKET_HELLO[2] = { '7','8' };

struct Vector3
{
	Vector3()
	{
		x = 0;
		y = 0;
		z = 0;
	}
	float x;
	float y;
	float z;
};

struct Transform
{
	Vector3 position;
};

class Client
{
public:
	Client(IPaddress address)
	{
		//remoteIP = new IPaddress();
		//remoteIP->host = address;
		//remoteIP->port = port;
		remoteIP = IPaddress(address);
		lastTime = SDL_GetTicks();
		id = maxClientID++;
	}

	bool operator== (const Client other)
	{
		if (other.remoteIP.host == remoteIP.host)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	//Transform transform;
	//Uint32 ipAddr = 0;
	//UDPsocket socket = 0;
	IPaddress remoteIP;
	unsigned int lastTime = 0;
	int id;
};

typedef std::vector<Client*> Clients;
Clients clients;


UDPsocket serverSocket;

 /* Pointer to packet memory */

void handleNewClients(Clients &clients, UDPpacket* p);
void handleNetworkData(Clients &clients, UDPpacket* p);
void handleTimeouts(Clients &clients);
bool isSameClient();

bool compareTypes(char a[2], char b[2])
{
	//printf("%c =/= %c\n", a[0], b[0]);
	//printf("%c =/= %c\n", a[1], b[1]);
	return (a[0] == b[0] && a[1] == b[1]);
}

void initServer()
{
	system("chcp 65001");
	std::locale::global(std::locale(""));
	//printf("Tt");

	if (SDL_Init(0) == -1) {
		printf("SDL_Init: %s\n", SDL_GetError());
		exit(1);
	}
	if (SDLNet_Init() == -1) {
		printf("SDLNet_Init: %s\n", SDLNet_GetError());
		exit(2);
	}

	serverSocket = SDLNet_UDP_Open(PORT);
	if (!serverSocket)
	{
		printf("SDLNet_UDP_Open: %s\n", SDLNet_GetError());
		exit(3);
	}

	// The main loop starts here
	printf("Server running at port %i, awaiting for packets\n", PORT);
}

char* processMessage(UDPpacket* p)
{
	//We're sent the whole stream of data from the Packet

	////GLOBAL STEPS
	//Step 1:
	// Check what type of a message it is with the prefix
	//Step 2:
	// Parse that into parts

	std::string mString = (char*)p->data;
	mString.resize(p->len);

	char packetType[2] = { mString[0], mString[1] };

	if (compareTypes(packetType, PACKET_SNAP))
	{
		//Step 3:
		// We need to return the glyph's position and the message string
		// 
		printf("Server Received SnapShot\n");
		return "Server Received ask for glyph!";
	}
	else if (compareTypes(packetType, PACKET_HIT))
	{
		//Step 3:
		// We need to save  the glyph's position and the message string
		// 
		printf("Server Received \n");
		return nullptr;
	}
	else
	{
		//std::cout << '\a';
		printf("Packet had unidentifiable or no prefix\n");
		return "Packet had unidentifiable or no prefix";
	}

	return nullptr;
}

int main(int argc, char **argv)
{
	initServer();
	UDPpacket* p = SDLNet_AllocPacket(1024);
	if (!p) {
		printf("SDLNet_AllocPacket: %s\n", SDLNet_GetError());
		exit(4);
	}

	bool running = true;

	while (running)
	{
		unsigned int currentTime = SDL_GetTicks();
		if (SDLNet_UDP_Recv(serverSocket, p) > 0)
		{
			printf("\nUDP Packet incoming\n");
			//printf("\tChan:  %d\n", p->channel);
			//printf("\tData: %*.*s %s", 0, p->len, (char*)p->data);
			//printf("\tData: %s", (char*)p->data);
			std::cout << "\tData: ";
			for (int i = 0; i < p->len; i++)
			{
				std::cout << (char)p->data[i];
			}
			std::cout << std::endl;
			printf("\tLen:  %d\n", p->len);
			p->data[p->len] = '\0';
			//printf("\tMaxlen:  %d\n", p->maxlen);
			//printf("\tStatus:  %d\n", p->status);
			printf("\tAddress: %u %u\n", p->address.host, p->address.port);
			handleNewClients(clients, p);
			handleNetworkData(clients, p);

		}

		handleTimeouts(clients);
		SDLNet_FreePacket(p);
		//Sleep(100);

		//for (int i = 0; i < MAX_USERS; i++)
		//{
		//	// Wait for a packet
		//	// UDP_Recv() returns != 0 if a packet is coming
		//	if (SDLNet_UDP_Recv(Clients[i].socket, p))
		//	{
		//		printf("\nUDP Packet incoming\n");
		//		//printf("\tChan:  %d\n", p->channel);
		//		//printf("\tData: %*.*s %s", 0, p->len, (char*)p->data);
		//		printf("\tData: %s", (char*)p->data);
		//		printf("\tLen:  %d\n", p->len);
		//		//printf("\tMaxlen:  %d\n", p->maxlen);
		//		//printf("\tStatus:  %d\n", p->status);
		//		//printf("\tAddress: %x %x\n", p->address.host, p->address.port);
	}

	//CleanUp();

	return 0;
}

void sendClientID(Client* c, UDPpacket* p)
{
	std::string packetType = PACKET_HELLO;
	std::string packetString = packetType + " " + std::to_string(c->id); 

	SDLNet_FreePacket(p);
	SDLNet_AllocPacket(1024);

	// Set the p->length to the size of string message + 1
	// And copy it to the p->data
	p->len = packetString.length();
	packetString.resize(p->len);

	memcpy(p->data, packetString.c_str(), p->len);

	printf("Sending message %s to %s:%s\n", packetString.c_str(), std::to_string(c->remoteIP.host).c_str(), std::to_string(c->remoteIP.port).c_str());
	p->address = c->remoteIP;
	// Send it
	SDLNet_UDP_Send(serverSocket, -1, p);

	SDLNet_FreePacket(p);
}

void handleNewClients(Clients &clients, UDPpacket* p)
{
	//printf("\nHandleNewClients() for \n\t>%u:%u\n", p->address.host, p->address.port);
	for each (Client* client in clients)
	{
		//printf("\t%u:%u\n", client->remoteIP.host, client->remoteIP.port);
		if (client->remoteIP.host == p->address.host && client->remoteIP.port == p->address.port)
		{
			// Break out of the method since there is 
			// already an existing Connection with this IPAddress

			// Also update the lastTime here
			client->lastTime = SDL_GetTicks();
			printf("\n\tClient's new LastTime is set to %u\n\n", SDL_GetTicks());
			return;
		}
	}

	Client* newClient = new Client(p->address);
	// Create new Client from the connection
	clients.push_back(newClient);
	printf("New connection found! Added to the Clients vector list\n");
	sendClientID(newClient, p);
}

void handleNetworkData(Clients &clients, UDPpacket* p)
{
	char packetType[2] = { (char)p->data[0], (char)p->data[1] };

	if (compareTypes(packetType, PACKET_SNAP))
	{
		// Got a Snapshot
		std::istringstream iss((char*)p->data);
		std::vector<std::string> results((std::istream_iterator<std::string>(iss)),
			std::istream_iterator<std::string>());

		Client* senderClient = 0;

		// Create a packet containing the following:
		// PacketType of SNAP | Client.ID | Position | RotationAngle
		for each (Client* client in clients)
		{
			// Get the client ID
			if (client->remoteIP.host == p->address.host && client->remoteIP.port == p->address.port)
			{
				senderClient = client;
				break;
			}
		}
		if (senderClient != nullptr)
		{
			std::string packetString = results[0] + " " + std::to_string(senderClient->id) + " " + results[2] + " " + results[3] + " " + results[4];

			SDLNet_FreePacket(p);
			SDLNet_AllocPacket(1024);

			// Set the p->length to the size of string message + 1
			// And copy it to the p->data
			p->len = packetString.length();
			packetString.resize(p->len);

			memcpy(p->data, packetString.c_str(), p->len);


			for each (Client* client in clients)
			{
				if (!(client->remoteIP.host == p->address.host && client->remoteIP.port == p->address.port))
				{
					printf("Sending message %s to %s:%s\n", packetString.c_str(), std::to_string(client->remoteIP.host).c_str(), std::to_string(client->remoteIP.port).c_str());
					p->address = client->remoteIP;
					// Send it
					SDLNet_UDP_Send(serverSocket, -1, p);
				}
			}
			SDLNet_FreePacket(p);
		}
	}
	//processMessage(p);
}

void handleTimeouts(Clients &clients)
{
	//printf("\tClientCount Before handleTimeouts: %i\n", clients.size());
	for(int i = 0; i < clients.size(); i++)
	{
		Client* client = clients[i];
		if (SDL_GetTicks() - client->lastTime > TIMEOUT_MS)
		{
			// Remove the Client from the vector
			//std::vector<Client>::iterator position = std::find(clients.begin(), clients.end(), client);
			//if (position != clients.end()) // == myVector.end() means the element was not found
			//	clients.erase(position);

			printf("=>Found a person who timed out!\n");
			printf("\tSDL_GetTicks() = %u\n", SDL_GetTicks());
			printf("\tUser last seen = %u\n", client->lastTime);

			clients.erase(
				std::remove_if(
					clients.begin(), clients.end(), [&](Client* const & _c) 
					{
						return _c->remoteIP.host == client->remoteIP.host && _c->remoteIP.port == client->remoteIP.port;
						//return _c.remoteIP.host == client.remoteIP.host;
					}),
					clients.end());
		}
	}
	//printf("\tClientCount After handleTimeouts: %i\n", clients.size());
}
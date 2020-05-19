set(SDL2_DIR ${CMAKE_SOURCE_DIR}/SDL2/SDL2/)
set(SDL2_image_DIR ${CMAKE_SOURCE_DIR}/SDL2/SDL2_image/)
set(SDL2_mixer_DIR ${CMAKE_SOURCE_DIR}/SDL2/SDL2_mixer/)
set(SDL2_net_DIR ${CMAKE_SOURCE_DIR}/SDL2/SDL2_net/)
set(SDL2_TTF_PATH ${CMAKE_SOURCE_DIR}/SDL2/SDL2_ttf/)
message(${SDL2_DIR})
message(${SDL2_image_DIR})
message(${SDL2_mixer_DIR})
message(${SDL2_net_DIR})
message(${SDL2_TTF_PATH})

macro (setup_server_exe)
	find_package(SDL2 REQUIRED)
	find_package(SDL2_net REQUIRED)
	include_directories(
	${SDL2_INCLUDE_DIRS} 
	${SDL2_NET_INCLUDE_DIR})
	LINK_DIRECTORIES(${SDL2_LIBRARIES} ${SDL2_NET_LIBRARY})
	find_library(${SDL2_LIBRARIES} ${SDL2_NET_LIBRARY})
	file (GLOB C_CPP_FILES src/*.cpp)
	file (GLOB C_H_FILES src/*.h)
	file (GLOB CPP_FILES src/Server/*.cpp)
	file (GLOB H_FILES src/Server/*.h)
	set (SOURCE_FILES ${CPP_FILES} ${H_FILES} ${C_CPP_FILES} ${C_H_FILES}) #Definning wild card and including all these
	add_executable(${SERVERNAME} ${SOURCE_FILES}) #Definning wild card and including all these
	target_link_libraries(${SERVERNAME} 
	${SDL2_LIBRARIES}  
	${SDL2_NET_LIBRARY}
	)
	file (GLOB SDL2_DLL_FILES ${SDL2_DIR}/lib/x86/*.dll)
	file (GLOB SDL2_net_DLL_FILES ${SDL2_net_DIR}/lib/x86/*.dll)
	
	set_property(TARGET ${SERVERNAME} PROPERTY VS_DEBUGGER_WORKING_DIRECTORY "${CMAKE_SOURCE_DIR}/Build/Debug")
	
	add_custom_command(
			TARGET ${SERVERNAME} POST_BUILD
			COMMAND ${CMAKE_COMMAND} -E copy
					${SDL2_DLL_FILES}
					${CMAKE_CURRENT_BINARY_DIR}/Debug/)
	add_custom_command(
			TARGET ${SERVERNAME} POST_BUILD
			COMMAND ${CMAKE_COMMAND} -E copy
					${SDL2_net_DLL_FILES}
					${CMAKE_CURRENT_BINARY_DIR}/Debug/)
endmacro()
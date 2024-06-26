START ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PDL.xml
XCOPY /Y GenPackets.cs "../../Dummy_Client/Packet"
XCOPY /Y GenPackets.cs "../../Tcp_Server/Packet"
XCOPY /Y ServerPacketManager.cs  "../../Tcp_Server/Packet"
XCOPY /Y ClientPacketManager.cs  "../../Dummy_Client/Packet"
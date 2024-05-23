To launch server : Tetris\TetrisServer\bin\Debug\TetrisServer.exe listening_port number_of_columns maximum_number_of_lines delay_speed
	Example : Tetris\TetrisServer\bin\Debug\TetrisServer.exe 4321 7 20 1000 

To launch player : Tetris\TetrisPlayer\bin\Debug\TetrisPlayer.exe server_address server_port high_key right_key low_key left_key 
	Example : Tetris\TetrisPlayer\bin\Debug\TetrisPlayer.exe 127.0.0.1 4321 Z D S Q 
	
	For the key, use this enumeration : https://msdn.microsoft.com/fr-fr/library/system.consolekey(v=vs.110).aspx
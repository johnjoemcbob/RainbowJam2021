<?php
include 'config.php';

// Pinged from Unity at the start of each player's session, to download all previous ghosts

if( true )
{
    $conn = new mysqli( DB_SERVER_NAME, DB_USER_NAME, DB_PW, DB_NAME, DB_PORT );
	if($conn->connect_error)
	{
        $conn->close();
        die( "Failed connection: " . $conn->connect_error );
	}
		// Get ghost paths in array
		$content = [];
		$q = sprintf( "SELECT * FROM %s", DB_TBL_GHOSTS );
		$res = $conn->query( $q );
			while( $row = $res->fetch_assoc() )
			{
				$content[] = $row["json"];
			}
		$res->close();

		// Return
		$result = array( "all" => $content );
		echo( json_encode( $result ) );
    $conn->close();
}
?>

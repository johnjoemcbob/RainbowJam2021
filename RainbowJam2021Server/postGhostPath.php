<?php
include 'config.php';

// Used by Unity to store the newly uploaded ghost path json string

$json = $_POST['json'];

if( isset( $json ) )
{
    $conn = new mysqli( DB_SERVER_NAME, DB_USER_NAME, DB_PW, DB_NAME, DB_PORT );
	if($conn->connect_error)
	{
        $conn->close();
        die( "Failed connection: " . $conn->connect_error );
	}
		// Insert this new path
		//$json = $conn->real_escape_string( $json );
		$q = sprintf( "INSERT INTO %s (json) VALUES ('$json')", DB_TBL_GHOSTS );
		$conn->query( $q );
    $conn->close();
}
?>

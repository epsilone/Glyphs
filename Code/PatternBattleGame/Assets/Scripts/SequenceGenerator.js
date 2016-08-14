#pragma strict

function Start () {

}

function generateSequence(sequence_len:int, plate_number:int)
{
	var _sequence:int[] = new int[sequence_len];
	// generate sequence
	for (var i = 0; i < sequence_len ; i++) {
		var j = Random.Range(0, plate_number);
		Debug.Log(i + "-" + j);
		_sequence[i] = j;
	}
	return _sequence;
}

function Update () {

}

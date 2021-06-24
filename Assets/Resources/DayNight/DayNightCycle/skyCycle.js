#pragma strict

/*DAY/NIGHT CYCLE SCRIPT
WRITTEN BY SAM BOYER
(so basically, please don't steal it :D)*/


//the secondsPerMinute changes the length of a minute. A lower value 
public var secondsPerMinute : float = 0.625; 

//starting time in hours, use decimal points for minutes
public var startTime : float = 12.0; 

//show date/time information?
public var showGUI : boolean = false;

//this variable is for the position of the area in degrees from the equator, therfore it must stay between 0 and 90.
//It determines now high the sun rises throughout the day, but not the length of the day yet.
public var latitudeAngle : float = 45.0;

//The transform component of the empty that tilts the sun's roataion.(the SunTilt object, not the Sun object itself)
public var sunTilt : Transform;


private var day : float;
private var min : float;
private var smoothMin : float;

private var texOffset : float;
private var skyMat : Material;
private var sunOrbit : Transform;

function Start () {
	skyMat = GetComponent(Renderer).sharedMaterial;
	sunOrbit = sunTilt.GetChild(0);

	sunTilt.eulerAngles.x = Mathf.Clamp(latitudeAngle,0,90); //set the sun tilt

	if(secondsPerMinute==0){
		Debug.LogError("Error! Can't have a time of zero, changed to 0.01 instead.");
		secondsPerMinute = 0.01;
	}
}

function UpdateSky(){
	smoothMin = (Time.time/secondsPerMinute) + (startTime*60);
	day = Mathf.Floor(smoothMin/1440)+1;

	smoothMin = smoothMin - (Mathf.Floor(smoothMin/1440)*1440); //clamp smoothMin between 0-1440
	min = Mathf.Round(smoothMin);

	sunOrbit.localEulerAngles.y = smoothMin/4;
	texOffset = Mathf.Cos((((smoothMin)/1440)*2)*Mathf.PI)*0.25+0.25;
	skyMat.mainTextureOffset = Vector2(Mathf.Round((texOffset-(Mathf.Floor(texOffset/360)*360))*1000)/1000,0);
}

function Update(){
	UpdateSky();
}

function OnGUI(){
	if(showGUI){
		GUI.Label(Rect(10,0,100,20),"Day "+day.ToString());
		GUI.Label(Rect(10,20,100,40),digitalDisplay(Mathf.Floor(min/60).ToString()) + ":" + digitalDisplay((min-Mathf.Floor(min/60)*60).ToString()));
	}
	//GUI.Label(Rect(10,40,100,60),texOffset.ToString()); //texture offset
}

function digitalDisplay(num : String){ //converts a number into a digital display (adds a zero if it's a single figure)
	if(num.Length==2){
		return num;
	}else{
		return "0"+num;
	}
}
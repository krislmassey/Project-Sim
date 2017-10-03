using UnityEngine;

public class ExampleSinglePlayerPaperScissorsRock : MonoBehaviour {

	public string player1Name;
	public GestureRecognition player1;

	public string player2Name;
	public GestureRecognition player2;

	bool gameInProgress;

	void OnEnable() {
		player1.OnGestureBegin.AddListener(BattleInitiated);
		player2.OnGestureBegin.AddListener(BattleInitiated);
	}

	void OnDisable() {
		player1.OnGestureBegin.RemoveListener(BattleInitiated);
		player2.OnGestureBegin.RemoveListener(BattleInitiated);
	}

	void BattleInitiated(GestureObject gesture) {
		if (!gameInProgress) {
			gameInProgress = true;
			Invoke("TheResultsAreIn",1.0f);
		}
	}

	void TheResultsAreIn() {
		switch (player1.currentGestureName) {
			case "Paper":
				switch (player2.currentGestureName) {
					case "Paper":
						Draw();
						break;
					case "Scissors":
						Victory(player2Name);
						break;
					case "Rock":
						Victory(player1Name);
						break;
					case "Unrecognised":
						Shenanigans(player2Name);
						break;
				}
				break;
			case "Scissors":
				switch (player2.currentGestureName) {
					case "Paper":
						Victory(player1Name);
						break;
					case "Scissors":
						Draw();
						break;
					case "Rock":
						Victory(player2Name);
						break;
					case "Unrecognised":
						Shenanigans(player2Name);
						break;
				}
				break;
			case "Rock":
				switch (player2.currentGestureName) {
					case "Paper":
						Victory(player2Name);
						break;
					case "Scissors":
						Victory(player1Name);
						break;
					case "Rock":
						Draw();
						break;
					case "Unrecognised":
						Shenanigans(player2Name);
						break;
				}
				break;
			case "Unrecognised":
				if (player2.currentGestureName == "Unrecognised") {
					Debug.Log("You're both cowards!");
				} else {
					Shenanigans(player1Name);
				}
				break;
		}
	}

	void Victory(string PlayerName) {
		switch (Random.Range(0,9)) {
			case 1:
				Debug.Log(PlayerName+" is quite possibly the greatest person alive.");
				break;
			case 2:
				Debug.Log(PlayerName+" is clearly the better opponent...");
				break;
			case 3:
				Debug.Log(PlayerName + " must've ate their weetbix.");
				break;
			case 4:
				Debug.Log("You know who's good? "+PlayerName+", clearly.");
				break;
			case 5:
				Debug.Log("I wish I was as cool as "+PlayerName);
				break;
			case 6:
				Debug.Log("When I grow up, I wanna be just like "+PlayerName);
				break;
			case 7:
				Debug.Log(PlayerName + " is off to the finals.");
				break;
			case 8:
				Debug.Log("Do you think " + PlayerName + " knows they're that good?");
				break;
			case 9:
				Debug.Log(PlayerName + " got good.");
				break;
		}

		gameInProgress = false;
	}

	void Draw() {
		Debug.Log("Draw, nobody wins. ");
		gameInProgress = false;
	}

	void Shenanigans(string whoWasThat) {
		switch (Random.Range(0,3)) {
			case 0:
				Debug.Log(whoWasThat + " is a big wuss, and probably has velcro shoelaces.");
				break;
			case 1:
				Debug.Log(whoWasThat + " is a big wuss, and probably buys lottery tickets.");
				break;
			case 2:
				Debug.Log(whoWasThat + " is a big wuss, and probably uses Bing.");
				break;
			case 3:
				Debug.Log(whoWasThat + " is a big wuss, and probably reads their junk mail.");
				break;
		}
	}
}

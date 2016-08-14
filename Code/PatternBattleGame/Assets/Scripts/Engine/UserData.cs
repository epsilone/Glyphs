using UnityEngine;
using System.Collections;

namespace com.bzr.puzzleBattle {

	public class UserData {

		public string userName;
		public long userId;
		public int unlockedLevel;

		// HighScore by level
		public HighScore[] highscores;

		// facebook
		public string facebookId;
		public string iosId;

		// matchs by friends
		public Match[] multiplayerMatchs;

		public UserData() {
			// TODO
		} 

		public int getUnlockedMove() {
			// TODO convert unlockedLevel to moveId;
			// Gord
			return this.unlockedLevel;
		}
	}

	public class UserController {
		private string dbuser = "pb-user";
		private string password = "bzrempire123";

		public UserData user;
		public UserController(UserData user){
			this.user = user;
		}

		public void populateMatchs() {
			// TODO contact the server to retrieve matches based on the userId
		}

		public void playMultiplayerMove(Match match, Move move) {
			// TODO
		}

		public void finishLevel(HighScore score) {
			// TODO save the score if it is a highscore.
			this.unlockLevel (score.level);
		}

		public void unlockLevel(int levelId) {
			if (levelId > this.user.unlockedLevel) {
				this.user.unlockedLevel = levelId;
			}
		}
	}

	public class HighScore {
		public int level;
		public int score;
		public long time;
		public long creationDate;
	}

	public class Match {
		public int matchId;
		public long opponentId;
		public string opponentName;
		public Sequence sequence;
		public bool isFinished;
		public bool isWin;
		public bool isPlayerTurn;
	}
}
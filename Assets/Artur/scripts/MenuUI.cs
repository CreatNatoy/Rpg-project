using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * Skrypt odpowiedzialny za zarządzanie głównym menu gry.
 * 
 * @author Hubert Paluch.
 * MViRe - na potrzeby kursu UNITY3D v5.
 * mvire.com 
 */
public class MenuUI : MonoBehaviour {

	public Canvas quitMenu;
	public Button btnStart;
	public Button btnExit;

	/** Obiekt menu.*/
	private Canvas manuUI;
	
	void Start (){
		manuUI = (Canvas)GetComponent<Canvas>();//Pobranie menu głównego.

		quitMenu = quitMenu.GetComponent<Canvas>(); //Pobranie menu pytania o wyjście z gry.

		btnStart = btnStart.GetComponent<Button> ();//Ustawienie przycisku uruchomienia gry.
		btnExit = btnExit.GetComponent<Button> ();//Ustawienie przycisku wyjścia z gry.

		quitMenu.enabled = false; //Ukrycie menu z pytaniem o wyjście z gry.

		Time.timeScale = 0;//Zatrzymanie czasu.
		Cursor.visible = manuUI.enabled;//Ukrycie pokazanie kursora myszy.
		Cursor.lockState = CursorLockMode.Confined;//Odblokowanie kursora myszy.
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.Escape)) { //Jeżeli naciśnięto klawisz "Escape"
			manuUI.enabled = !manuUI.enabled;//Ukrycie/pokazanie menu.

			Cursor.visible = manuUI.enabled;//Ukrycie pokazanie kursora myszy.
			
			if(manuUI.enabled) {
				Cursor.lockState = CursorLockMode.Confined;//Odblokowanie kursora myszy.
				Cursor.visible = true;//Pokazanie kursora.
				Time.timeScale = 0;//Zatrzymanie czasu.
				quitMenu.enabled = false; //Ukrycie menu pytania.
				btnStart.enabled = true; //Aktywacja przycsiku 'Start'.
				btnExit.enabled = true; //Aktywacja przycsiku 'Wyjście'.
			} else {
				Cursor.lockState = CursorLockMode.Locked; //Zablokowanie kursora myszy.
				Cursor.visible = false;//Ukrycie kursora.
				Time.timeScale = 1;//Włączenie czasu.
				quitMenu.enabled = false; //Ukrycie menu pytania.
			}
			
		}
	}

	//Metoda wywoływana po naciśnięciu przycisku "Exit"
	public void PrzyciskWyjscie() {
		quitMenu.enabled = true; //Uaktywnienie meny z pytaniem o wyjście
		btnStart.enabled = false; //Deaktywacja przycsiku 'Start'.
		btnExit.enabled = false; //Deaktywacja przycsiku 'Wyjście'.
	}

	//Metoda wywoływana podczas udzielenia odpowiedzi przeczącej na pytanie o wyjście z gry.
	public void PrzyciskNieWychodz(){
		quitMenu.enabled = false; //Ukrycie menu z pytaniem o wyjście z gry.
		btnStart.enabled = true; //Uaktywnienie przycisku 'Start'.
		btnExit.enabled = true; //Uaktywnienie przycisku 'Wyjscie'.
	}

	//Metoda wywoływana przez przycisk uruchomienia gry 'Play Game'
	public void PrzyciskStart (){
		//Application.LoadLevel (0); //this will load our first level from our build settings. "1" is the second scene in our game	
		manuUI.enabled = false; //Ukrycie głównego menu.

		Time.timeScale = 1;//Właczenie czasu.

		Cursor.visible = false;//Ukrycie kursora.
		Cursor.lockState = CursorLockMode.Locked; //Zablokowanie kursora myszy.
	}

	//Metoda wywoływana podczas udzielenia odpowiedzi twierdzącej na pytanie o wyjście z gry.
	public void PrzyciskTakWyjdz () {
		Application.Quit(); //Powoduje wyjście z gry.
		
	}
}

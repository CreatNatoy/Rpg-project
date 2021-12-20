using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * Skrypt odpowiedzialny za zarz�dzanie g��wnym menu gry.
 */
public class MenuUI : MonoBehaviour {

	public Canvas quitMenu;
	public Canvas controlMenu;
	public Button btnStart;
	public Button btnControl;
	public Button btnExit;
	public Button btnControlExit;

	/** Obiekt menu.*/
	private Canvas manuUI;
	
	void Start (){
		manuUI = (Canvas)GetComponent<Canvas>();//Pobranie menu g��wnego.
		controlMenu = controlMenu.GetComponent<Canvas>(); //Pobranie menu kontroli gry.
		quitMenu = quitMenu.GetComponent<Canvas>();  //Pobranie menu pytania o wyj�cie z gry.

		btnStart = btnStart.GetComponent<Button> ();//Ustawienie przycisku uruchomienia gry.
		btnControl = btnControl.GetComponent<Button>();//Ustawienie przycisku uruchomienia opcji.
		btnExit = btnExit.GetComponent<Button> ();//Ustawienie przycisku wyj�cia z gry.
		btnControlExit = btnControlExit.GetComponent<Button>();//Ustawienie przycisku wyj�cia z gry.

		controlMenu.enabled = false; //Ukrycie menu z kontrolowaniem.
		quitMenu.enabled = false; //Ukrycie menu z pytaniem o wyj�cie z gry.

		Time.timeScale = 0;//Zatrzymanie czasu.
		Cursor.visible = manuUI.enabled;//Ukrycie pokazanie kursora myszy.
		Cursor.lockState = CursorLockMode.Confined;//Odblokowanie kursora myszy.
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{ //Je�eli naci�ni�to klawisz "Escape"
			manuUI.enabled = !manuUI.enabled;//Ukrycie/pokazanie menu.

			Cursor.visible = manuUI.enabled;//Ukrycie pokazanie kursora myszy.

			if (manuUI.enabled)
			{
				Cursor.lockState = CursorLockMode.Confined;//Odblokowanie kursora myszy.
				Cursor.visible = true;//Pokazanie kursora.
				Time.timeScale = 0;//Zatrzymanie czasu.
				quitMenu.enabled = false; //Ukrycie menu pytania.
				btnStart.enabled = true; //Aktywacja przycsiku 'Start'.
				btnExit.enabled = true; //Aktywacja przycsiku 'Wyj�cie'.
			}
			else
			{
				Cursor.lockState = CursorLockMode.Locked; //Zablokowanie kursora myszy.
				Cursor.visible = false;//Ukrycie kursora.
				Time.timeScale = 1;//W��czenie czasu.
				quitMenu.enabled = false; //Ukrycie menu pytania.
			}

		}
	}

	//Metoda wywo�ywana po naci�ni�ciu przycisku "Control"
	public void PrzyciskControl()
	{
		controlMenu.enabled = true; //Uaktywnienie menu control.
		btnStart.enabled = false; //Deaktywacja przycsiku 'Start'.
		btnControl.enabled = false; //Deaktywacja przycsiku 'Control'.
		btnExit.enabled = false; //Deaktywacja przycsiku 'Wyj�cie'.
	}

	//Metoda wywo�ywana po naci�ni�ciu przycisku "ExitControl"
	public void PrzyciskExitControl()
	{
		controlMenu.enabled = false; //Uaktywnienie menu control.
		btnStart.enabled = true; //Uaktywnienie przycisku 'Start'.
		btnControl.enabled = true; //Uaktywnienie przycsiku 'Control'.
		btnExit.enabled = true; //Uaktywnienie przycisku 'Wyjscie'.
	}


	//Metoda wywo�ywana po naci�ni�ciu przycisku "Exit"
	public void PrzyciskWyjscie()
	{
		quitMenu.enabled = true; //Uaktywnienie meny z pytaniem o wyj�cie
		btnStart.enabled = false; //Deaktywacja przycsiku 'Start'.
		btnControl.enabled = false; //Deaktywacja przycsiku 'Control'.
		btnExit.enabled = false; //Deaktywacja przycsiku 'Wyj�cie'.
	}


	//Metoda wywo�ywana podczas udzielenia odpowiedzi przecz�cej na pytanie o wyj�cie z gry.
	public void PrzyciskNieWychodz(){
		quitMenu.enabled = false; //Ukrycie menu z pytaniem o wyj�cie z gry.
		btnStart.enabled = true; //Uaktywnienie przycisku 'Start'.
		btnControl.enabled = true; //Uaktywnienie przycsiku 'Control'.
		btnExit.enabled = true; //Uaktywnienie przycisku 'Wyjscie'.
	}

	//Metoda wywo�ywana przez przycisk uruchomienia gry 'Play Game'
	public void PrzyciskStart (){
		//Application.LoadLevel (0); //this will load our first level from our build settings. "1" is the second scene in our game	
		manuUI.enabled = false; //Ukrycie g��wnego menu.

		Time.timeScale = 1;//W�aczenie czasu.

		Cursor.visible = false;//Ukrycie kursora.
		Cursor.lockState = CursorLockMode.Locked; //Zablokowanie kursora myszy.
	}

	//Metoda wywo�ywana podczas udzielenia odpowiedzi twierdz�cej na pytanie o wyj�cie z gry.
	public void PrzyciskTakWyjdz () {
		Application.Quit(); //Powoduje wyj�cie z gry.
		
	}
}

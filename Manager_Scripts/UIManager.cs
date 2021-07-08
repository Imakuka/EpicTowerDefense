using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;



namespace GameDevHQ.TowerDefense
{
    public class UIManager : MonoSingleton<UIManager>
    {

        public static event Action<bool> onTurretUpgradeMode;
        public static event Action<int, GameObject> onTurretSale;

        [Header("Turrets")]
        [SerializeField]
        private Turret[] _theTurrets;//Gatling Gun = 0, Dual Gatling Gun = 1, Missle Launcher = 2, Dual AdvancedMissile Launcher = 3
        [Header("UI Color")]
        [SerializeField]
        private Image[] _uiImages;
        [SerializeField]
        private Color[] _colors;//0 = blue, 1 = yellow, 2 = red

        [Header("Playback")]
        [SerializeField]
        private Button[] _playModeButtons;//Play = 0, Pause = 1, Fast Forward = 2
        [SerializeField]
        private Image[] _playModeImages;//Play = 0, Pause = 1, Fast Forward = 2
        [SerializeField]
        private Button _playButton;
        [SerializeField]
        private Image _playImage;
        [SerializeField]
        private bool _inPlayMode = false;
        [SerializeField]
        private bool _gamePaused = false;
        [SerializeField]
        private bool _fastForwardMode = false;

        [Header("Level Status")]
        [SerializeField]
        private Image _anouncementImage;
        [SerializeField]
        private Text[] _anouncementTexts;//Welcome Text = 0, Start Level = 1, Countdown = 2, Level Complete = 3

        [Header("Restart")]
        [SerializeField]
        private Image _restartImage;

        [Header("Destroyed & Waves Text")]
        [SerializeField]
        private Text[] _counterText;//Player Lives = 0, Current wave = 1

        [Header("War Funds")] //this should display the whole time?
        [SerializeField]
        private Text _warFundsAmountText;
        [SerializeField]
        private int _warFundsAmount;
        [SerializeField]
        private Text _statusValue;

        [Header("Armory")]
        [SerializeField]
        private Image[] _armoryImages;//Gatling Gun = 0, Dual Gatling Gun = 1, Missle Launcher = 2, Dual AdvancedMissile Launcher = 3
        [SerializeField]
        private Button[] _armoryButtons;//Gatling Gun = 0, Dual Gatling Gun = 1, Missle Launcher = 2, Dual AdvancedMissile Launcher = 3
        [SerializeField]
        private Text[] _weaponCostText;//Gatling Gun = 0, Dual Gatling Gun = 1, Missle Launcher = 2, Dual AdvancedMissile Launcher = 3

        [Header("Upgrade Mode")]
        [SerializeField]
        private Button[] _upgradeButtons;//UpgradeButton = 0, Sell Button = 1, Dont Sell = 2
        [SerializeField]
        private Image _sellTurretImage;
        [SerializeField]
        private Text _profitText;
        [SerializeField]
        private Text _upgradeText;
        [SerializeField]
        private bool _upgradeMode;
        [SerializeField]
        private int _currentTurretType;

        private int _turretProfitAmount;
        private GameObject _turretLocaation;

        private WaitForSeconds _beginingGameYield;
        private float _beginingOffset = 4.0f;
        private WaitForSeconds _countdownFlickerYield;
        private float _countdownOffset = 1.0f;
        private WaitForSeconds _lostLifeYield;
        private float _lostLifeOffset = 3.0f;


        void Start()
        {
            DefineComponents();
            BasicArmoryButtons();
            ChangeUIColor(0);
        }

        public void ChangeUIColor(int colorNumber)
        {

            for (int i = 0; i < _uiImages.Length; i++)
            {
                _uiImages[i].color = _colors[colorNumber];
            }
        }

        private void DefineComponents()
        {
            for (int i = 0; i < _weaponCostText.Length; i++)
            {
                for (int j = 0; j < _theTurrets.Length; j++)
                {
                    _weaponCostText[i].text = _theTurrets[i].warFundsCost.ToString("");
                }
            }

            _beginingGameYield = new WaitForSeconds(_beginingOffset);
            _countdownFlickerYield = new WaitForSeconds(_countdownOffset);
            _lostLifeYield = new WaitForSeconds(_lostLifeOffset);
        }

        public IEnumerator BeginingOfGameRoutine()
        {
            _anouncementImage.gameObject.SetActive(true);
            _anouncementTexts[0].gameObject.SetActive(true);
            yield return _beginingGameYield;
            _anouncementImage.gameObject.SetActive(false);
            _playModeImages[0].gameObject.SetActive(true);
            _anouncementTexts[0].gameObject.SetActive(false);
            _armoryImages[0].gameObject.SetActive(true);
            _armoryImages[2].gameObject.SetActive(true);
        }

        public void StartLevel()
        {
            if (_inPlayMode == false)
            {
                StartCoroutine(StartLevelRoutine());
            }

        }
        private IEnumerator StartLevelRoutine()
        {
            _playModeButtons[0].interactable = false;
            _playModeImages[0].gameObject.SetActive(false);
            _anouncementImage.gameObject.SetActive(true);
            _anouncementTexts[1].gameObject.SetActive(true);
            _anouncementTexts[2].gameObject.SetActive(true);
            _anouncementTexts[2].text = ("5");
            yield return _countdownFlickerYield;
            _armoryImages[0].gameObject.SetActive(false);
            _armoryImages[2].gameObject.SetActive(false);
            _upgradeButtons[0].gameObject.SetActive(false);
            _anouncementTexts[2].text = ("4");
            yield return _countdownFlickerYield;
            _anouncementTexts[2].text = ("3");
            yield return _countdownFlickerYield;
            _anouncementTexts[2].text = ("2");
            yield return _countdownFlickerYield;
            _anouncementTexts[2].text = ("1");
            yield return _countdownFlickerYield;
            _anouncementImage.gameObject.SetActive(false);
            _anouncementTexts[1].gameObject.SetActive(false);
            _anouncementTexts[2].gameObject.SetActive(false);

            SpawnManager.Instance.CanSpawn(true);
            PlayMode(true);
        }

        public void UpdatePlayerLivesCount(int livesRemaining)
        {
            _counterText[0].text = ("" + livesRemaining);
        }

        public void UpdateWavesCount(int wave)
        {
            int currentWave = wave + 1;
            _counterText[1].text = ("" + currentWave);
        }
        public void UpdateWarFundsAmount(int funds)
        {
            _warFundsAmountText.text = ("" + funds);
            _warFundsAmount = funds;
            if (funds > 500)
            {
                _statusValue.color = Color.green;
            }
            if (funds < 500)
            {
                _statusValue.text = (" Low Funds");
                _statusValue.color = Color.red;

            }
            if (funds == 0)
            {
                _statusValue.text = (" NO FUNDS");
                _statusValue.color = Color.red;
            }

        }

        private void BasicArmoryButtons()
        {
            if (_warFundsAmount < _theTurrets[0].warFundsCost)
            {
                _armoryButtons[0].interactable = false;
                _armoryButtons[2].interactable = false;
            }

            if (_warFundsAmount < _theTurrets[2].warFundsCost)
            {
                _armoryButtons[2].interactable = false;
            }

        }

        public void UpgradeButton()
        {
            _upgradeButtons[0].gameObject.SetActive(true);//Activates the Upgrade Button
        }
        public void OnTurretUpgradeMode(bool upgrading)
        {
            if (onTurretUpgradeMode != null)
            {
                onTurretUpgradeMode(upgrading); //Event saying We are in Upgrade Mode for anyone listening
            }
            if (upgrading == true)
            {
                _upgradeButtons[0].gameObject.SetActive(false);
            }

        }
        public void TurretUpgradeButtons(bool active)
        {
            if (active == true)
            {
                if (_warFundsAmount >= _theTurrets[1].warFundsCost)
                {
                    _armoryImages[1].gameObject.SetActive(true);
                }

                if (_warFundsAmount >= _theTurrets[3].warFundsCost)
                {
                    _armoryImages[3].gameObject.SetActive(true);
                }
            }
            else
            {
                _armoryImages[1].gameObject.SetActive(false);
                _armoryImages[3].gameObject.SetActive(false);
            }

        }
        
        public void SellTurret()
        {
            if (onTurretSale != null)
            {
                onTurretSale(_turretProfitAmount, _turretLocaation);//Event saying a turret was sold at this location
            }
            _sellTurretImage.gameObject.SetActive(false);
            TurretUpgradeButtons(true);
            OnTurretUpgradeMode(false);
        }

        public void DontSellTurret()
        {
            _sellTurretImage.gameObject.SetActive(false);
        }

        public void UpgradingTurret(GameObject location, int turretType)//Recieving info from Tower spots
        {
            Debug.Log("UiManager:: UpgradingTurret: Method Called");
            Debug.Log("UiManager:: UpgradingTurret: Current Turret " + turretType);
            _currentTurretType = turretType;
            _turretLocaation = location;
            _sellTurretImage.gameObject.SetActive(true);
            _turretProfitAmount = _theTurrets[_currentTurretType].warFundsCost - 100;
            _profitText.text = _turretProfitAmount.ToString("");
        }

        public void WaveComplete(bool waveComplete)
        {
            if (waveComplete == true)
            {
                PlayMode(false);
                _anouncementImage.gameObject.SetActive(true);
                _anouncementTexts[3].gameObject.SetActive(true);
                _armoryImages[0].gameObject.SetActive(true);
                _armoryImages[2].gameObject.SetActive(true);
                _upgradeButtons[0].gameObject.SetActive(true);
            }
            else
            {
                _anouncementImage.gameObject.SetActive(false);
                _anouncementTexts[3].gameObject.SetActive(false);
            }
            
        }

        public void PlayMode(bool playing)
        {
            _inPlayMode = playing;
            if (_inPlayMode == true)
            {

                _playModeButtons[0].interactable = false;
                _playModeImages[0].gameObject.SetActive(false);
                _playModeImages[1].gameObject.SetActive(true);
                _playModeImages[2].gameObject.SetActive(true);
                _playModeButtons[1].interactable = true;
                _playModeButtons[2].interactable = true;

            }
            if (_gamePaused == false)
            {
                _armoryButtons[0].interactable = false;
                _armoryButtons[2].interactable = false;
            }

        }
        public void PauseMode(bool paused)
        {
            _gamePaused = paused;
            if (_gamePaused == true)
            {
                _playModeButtons[1].interactable = false;
                _playModeImages[1].gameObject.SetActive(false);
                _playModeImages[0].gameObject.SetActive(true);
                _playModeButtons[0].interactable = true;
                _playModeImages[2].gameObject.SetActive(true);
                _playModeButtons[2].interactable = true;
                _armoryImages[0].gameObject.SetActive(true);
                _armoryImages[2].gameObject.SetActive(true);
                _armoryButtons[0].interactable = true;
                _armoryButtons[2].interactable = true;
                _upgradeButtons[0].gameObject.SetActive(true);
            }

        }

        public void FastForwardMode(bool fastMode)
        {
            _fastForwardMode = fastMode;
            if (_fastForwardMode == true)
            {
                _playModeImages[2].gameObject.SetActive(false);
                _playModeButtons[2].interactable = false;
                _playModeButtons[0].interactable = true;
                _playModeImages[0].gameObject.SetActive(true);
                _playModeImages[1].gameObject.SetActive(true);
                _playModeButtons[1].interactable = true;
            }

        }

        public void LostALife()
        {
            StartCoroutine(LostLifeRoutine());
        }
        public void Restart(bool restartMode)// move to gamem
        {

            if (restartMode == true)
            {
                _restartImage.gameObject.SetActive(true);
                _inPlayMode = false;       
            }

        }
        IEnumerator LostLifeRoutine()
        {
            _anouncementImage.gameObject.SetActive(true);
            _anouncementTexts[4].gameObject.SetActive(true);
            yield return _lostLifeYield;
            _anouncementImage.gameObject.SetActive(false);
            _anouncementTexts[4].gameObject.SetActive(false);

        }


    }
}


﻿using System;
using System.Reflection;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Emilie.UWP.Utilities
{
    /// <summary>
    /// Common character codes for Symbols inside the Segoe MDL2 font
    /// In XAML, using like : "&#xE---";
    /// </summary>
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public class MDL2 : MarkupExtension
    {
        /*
         * The following values are parsed directly from the HTML of
         * https://docs.microsoft.com/en-us/windows/uwp/design/style/segoe-ui-symbol-font.
         * See end of file for example parsing code
         */

        #region Char mappings

        public const char Accept = '\uE8FB';
        public const char Accident = '\uE81F';
        public const char AccidentSolid = '\uEA8E';
        public const char Account = '\uE168';
        public const char Accounts = '\uE910';
        public const char ActionCenter = '\uE91C';
        public const char ActionCenterAsterisk = '\uEA21';
        public const char ActionCenterMirrored = '\uED0D';
        public const char ActionCenterNotification = '\uE7E7';
        public const char ActionCenterNotificationMirrored = '\uED0C';
        public const char ActionCenterQuiet = '\uEE79';
        public const char ActionCenterQuietNotification = '\uEE7A';
        public const char Add = '\uE710';
        public const char AddFriend = '\uE8FA';
        public const char AddRemoteDevice = '\uE836';
        public const char AddSurfaceHub = '\uECC4';
        public const char AddTo = '\uECC8';
        public const char AdjustHologram = '\uEBD2';
        public const char Admin = '\uE7EF';
        public const char Airplane = '\uE709';
        public const char AirplaneSolid = '\uEB4C';
        public const char AlignCenter = '\uE8E3';
        public const char AlignLeft = '\uE8E4';
        public const char AlignRight = '\uE8E2';
        public const char AllApps = '\uE71D';
        public const char AllAppsMirrored = '\uEA40';
        public const char Annotation = '\uE924';
        public const char AppIconDefault = '\uECAA';
        public const char ArrowDown8 = '\uF0AE';
        public const char ArrowLeft8 = '\uF0B0';
        public const char ArrowRight8 = '\uF0AF';
        public const char ArrowUp8 = '\uF0AD';
        public const char AspectRatio = '\uE799';
        public const char Asterisk = '\uEA38';
        public const char AsteriskBadge12 = '\uEDAD';
        public const char Attach = '\uE723';
        public const char AttachCamera = '\uE8A2';
        public const char Audio = '\uE8D6';
        public const char Back = '\uE72B';
        public const char BackgroundToggle = '\uEF1F';
        public const char BackMirrored = '\uF0D2';
        public const char BackSpaceQWERTY = '\uE750';
        public const char BackSpaceQWERTYLg = '\uEB96';
        public const char BackSpaceQWERTYMd = '\uE926';
        public const char BackSpaceQWERTYSm = '\uE925';
        public const char BackToWindow = '\uE73F';
        public const char Badge = '\uEC1B';
        public const char Bank = '\uE825';
        public const char BarcodeScanner = '\uEC5A';
        public const char Battery0 = '\uE850';
        public const char Battery1 = '\uE851';
        public const char Battery10 = '\uE83F';
        public const char Battery2 = '\uE852';
        public const char Battery3 = '\uE853';
        public const char Battery4 = '\uE854';
        public const char Battery5 = '\uE855';
        public const char Battery6 = '\uE856';
        public const char Battery7 = '\uE857';
        public const char Battery8 = '\uE858';
        public const char Battery9 = '\uE859';
        public const char BatteryCharging0 = '\uE85A';
        public const char BatteryCharging1 = '\uE85B';
        public const char BatteryCharging10 = '\uEA93';
        public const char BatteryCharging2 = '\uE85C';
        public const char BatteryCharging3 = '\uE85D';
        public const char BatteryCharging4 = '\uE85E';
        public const char BatteryCharging5 = '\uE85F';
        public const char BatteryCharging6 = '\uE860';
        public const char BatteryCharging7 = '\uE861';
        public const char BatteryCharging8 = '\uE862';
        public const char BatteryCharging9 = '\uE83E';
        public const char BatterySaver0 = '\uE863';
        public const char BatterySaver1 = '\uE864';
        public const char BatterySaver10 = '\uEA95';
        public const char BatterySaver2 = '\uE865';
        public const char BatterySaver3 = '\uE866';
        public const char BatterySaver4 = '\uE867';
        public const char BatterySaver5 = '\uE868';
        public const char BatterySaver6 = '\uE869';
        public const char BatterySaver7 = '\uE86A';
        public const char BatterySaver8 = '\uE86B';
        public const char BatterySaver9 = '\uEA94';
        public const char BatteryUnknown = '\uE996';
        public const char Beta = '\uEA24';
        public const char BidiLtr = '\uE9AA';
        public const char BidiRtl = '\uE9AB';
        public const char BlockContact = '\uE8F8';
        public const char BlueLight = '\uF08C';
        public const char Bluetooth = '\uE702';
        public const char BodyCam = '\uEC80';
        public const char Bold = '\uE8DD';
        public const char Bookmarks = '\uE8A4';
        public const char BookmarksMirrored = '\uEA41';
        public const char Brightness = '\uE706';
        public const char Broom = '\uEA99';
        public const char BrowsePhotos = '\uE7C5';
        public const char BrushSize = '\uEDA8';
        public const char BuildingEnergy = '\uEC0B';
        public const char BulletedList = '\uE8FD';
        public const char BulletedListMirrored = '\uEA42';
        public const char Bullets = '\uE133';
        public const char Bullseye = '\uF272';
        public const char BumperLeft = '\uF10C';
        public const char BumperRight = '\uF10D';
        public const char Bus = '\uE806';
        public const char BusSolid = '\uEB47';
        public const char ButtonA = '\uF093';
        public const char ButtonB = '\uF094';
        public const char ButtonX = '\uF096';
        public const char ButtonY = '\uF095';
        public const char Calculator = '\uE8EF';
        public const char CalculatorAddition = '\uE948';
        public const char CalculatorBackspace = '\uE94F';
        public const char CalculatorDivide = '\uE94A';
        public const char CalculatorEqualTo = '\uE94E';
        public const char CalculatorMultiply = '\uE947';
        public const char CalculatorNegate = '\uE94D';
        public const char CalculatorPercentage = '\uE94C';
        public const char CalculatorSquareroot = '\uE94B';
        public const char CalculatorSubtract = '\uE949';
        public const char Calendar = '\uE787';
        public const char CalendarDay = '\uE8BF';
        public const char CalendarMirrored = '\uED28';
        public const char CalendarReply = '\uE8F5';
        public const char CalendarSolid = '\uEA89';
        public const char CalendarWeek = '\uE8C0';
        public const char CaligraphyPen = '\uEDFB';
        public const char CallForwarding = '\uE7F2';
        public const char CallForwardingMirrored = '\uEA97';
        public const char CallForwardInternational = '\uE87A';
        public const char CallForwardInternationalMirrored = '\uEA43';
        public const char CallForwardRoaming = '\uE87B';
        public const char CallForwardRoamingMirrored = '\uEA44';
        public const char CalligraphyFill = '\uF0C7';
        public const char Camera = '\uE722';
        public const char Cancel = '\uE711';
        public const char Caption = '\uE8BA';
        public const char Car = '\uE804';
        public const char CaretBottomRightSolidCenter8 = '\uF169';
        public const char CashDrawer = '\uEC59';
        public const char CC = '\uE7F0';
        public const char CellPhone = '\uE8EA';
        public const char Certificate = '\uEB95';
        public const char Character = '\uE164';
        public const char CharacterAppearance = '\uF17F';
        public const char Characters = '\uE8C1';
        public const char ChatBubbles = '\uE8F2';
        public const char Checkbox = '\uE739';
        public const char Checkbox14 = '\uF16B';
        public const char CheckboxComposite = '\uE73A';
        public const char CheckboxComposite14 = '\uF16C';
        public const char CheckboxCompositeReversed = '\uE73D';
        public const char CheckboxFill = '\uE73B';
        public const char CheckboxIndeterminate = '\uE73C';
        public const char CheckboxIndeterminateCombo = '\uF16E';
        public const char CheckboxIndeterminateCombo14 = '\uF16D';
        public const char ChecklistMirrored = '\uF0B5';
        public const char CheckMark = '\uE73E';
        public const char ChevronDown = '\uE70D';
        public const char ChevronDownMed = '\uE972';
        public const char ChevronDownSmall = '\uE96E';
        public const char ChevronLeft = '\uE76B';
        public const char ChevronLeftMed = '\uE973';
        public const char ChevronLeftSmall = '\uE96F';
        public const char ChevronRight = '\uE76C';
        public const char ChevronRightMed = '\uE974';
        public const char ChevronRightSmall = '\uE970';
        public const char ChevronUp = '\uE70E';
        public const char ChevronUpMed = '\uE971';
        public const char ChevronUpSmall = '\uE96D';
        public const char ChineseBoPoMoFo = '\uE989';
        public const char ChineseChangjie = '\uE981';
        public const char ChinesePinyin = '\uE98A';
        public const char ChinesePunctuation = '\uF111';
        public const char ChineseQuick = '\uE984';
        public const char ChromeAnnotate = '\uE931';
        public const char ChromeAnnotateContrast = '\uF0F9';
        public const char ChromeBack = '\uE830';
        public const char ChromeBackContrast = '\uF0D5';
        public const char ChromeBackContrastMirrored = '\uF0D6';
        public const char ChromeBackMirrored = '\uEA47';
        public const char ChromeBackToWindow = '\uE92C';
        public const char ChromeBackToWindowContrast = '\uF0D7';
        public const char ChromeClose = '\uE8BB';
        public const char ChromeFullScreen = '\uE92D';
        public const char ChromeFullScreenContrast = '\uF0D8';
        public const char ChromeMaximize = '\uE922';
        public const char ChromeMinimize = '\uE921';
        public const char ChromeRestore = '\uE923';
        public const char ChromeSwitch = '\uF1CB';
        public const char ChromeSwitchContast = '\uF1CC';
        public const char CircleFill = '\uEA3B';
        public const char CircleFillBadge12 = '\uEDB0';
        public const char CircleRing = '\uEA3A';
        public const char CircleRingBadge12 = '\uEDAF';
        public const char CityNext = '\uEC06';
        public const char CityNext2 = '\uEC07';
        public const char Clear = '\uE894';
        public const char ClearAllInk = '\uED62';
        public const char ClearAllInkMirrored = '\uEF19';
        public const char ClearSelection = '\uE8E6';
        public const char ClearSelectionMirrored = '\uEA48';
        public const char Click = '\uE8B0';
        public const char ClipboardList = '\uF0E3';
        public const char ClipboardListMirrored = '\uF0E4';
        public const char Clock = '\uE121';
        public const char ClosedCaption = '\uE190';
        public const char ClosePane = '\uE89F';
        public const char ClosePaneMirrored = '\uEA49';
        public const char Cloud = '\uE753';
        public const char CloudPrinter = '\uEDA6';
        public const char Code = '\uE943';
        public const char CollapseContent = '\uF165';
        public const char CollapseContentSingle = '\uF166';
        public const char Color = '\uE790';
        public const char CommaKey = '\uE9AD';
        public const char CommandPrompt = '\uE756';
        public const char Comment = '\uE90A';
        public const char Communications = '\uE95A';
        public const char CompanionApp = '\uEC64';
        public const char CompanionDeviceFramework = '\uED5D';
        public const char Completed = '\uE930';
        public const char CompletedSolid = '\uEC61';
        public const char Component = '\uE950';
        public const char Connect = '\uE703';
        public const char ConnectApp = '\uED5C';
        public const char Connected = '\uF0B9';
        public const char Construction = '\uE822';
        public const char ConstructionCone = '\uE98F';
        public const char ConstructionSolid = '\uEA8D';
        public const char Contact = '\uE77B';
        public const char Contact2 = '\uE8D4';
        public const char ContactInfo = '\uE779';
        public const char ContactInfoMirrored = '\uEA4A';
        public const char ContactPresence = '\uE8CF';
        public const char ContactSolid = '\uEA8C';
        public const char Copy = '\uE8C8';
        public const char Courthouse = '\uEC08';
        public const char Crop = '\uE7A8';
        public const char Cut = '\uE8C6';
        public const char DashKey = '\uE9AE';
        public const char DataSense = '\uE791';
        public const char DataSenseBar = '\uE7A5';
        public const char DateTime = '\uEC92';
        public const char DateTimeMirrored = '\uEE93';
        public const char DefaultAPN = '\uF080';
        public const char DefenderApp = '\uE83D';
        public const char DefenderBadge12 = '\uF0FB';
        public const char Delete = '\uE74D';
        public const char DetachablePC = '\uF103';
        public const char DeveloperTools = '\uEC7A';
        public const char DeviceDiscovery = '\uEBDE';
        public const char DeviceLaptopNoPic = '\uE7F8';
        public const char DeviceLaptopPic = '\uE7F7';
        public const char DeviceMonitorLeftPic = '\uE7FA';
        public const char DeviceMonitorNoPic = '\uE7FB';
        public const char DeviceMonitorRightPic = '\uE7F9';
        public const char Devices = '\uE772';
        public const char Devices2 = '\uE975';
        public const char Devices3 = '\uEA6C';
        public const char Devices4 = '\uEB66';
        public const char DevUpdate = '\uECC5';
        public const char Diagnostic = '\uE9D9';
        public const char Dial1 = '\uF146';
        public const char Dial10 = '\uF14F';
        public const char Dial11 = '\uF150';
        public const char Dial12 = '\uF151';
        public const char Dial13 = '\uF152';
        public const char Dial14 = '\uF153';
        public const char Dial15 = '\uF154';
        public const char Dial16 = '\uF155';
        public const char Dial2 = '\uF147';
        public const char Dial3 = '\uF148';
        public const char Dial4 = '\uF149';
        public const char Dial5 = '\uF14A';
        public const char Dial6 = '\uF14B';
        public const char Dial7 = '\uF14C';
        public const char Dial8 = '\uF14D';
        public const char Dial9 = '\uF14E';
        public const char Dialpad = '\uE75F';
        public const char DialShape1 = '\uF156';
        public const char DialShape2 = '\uF157';
        public const char DialShape3 = '\uF158';
        public const char DialShape4 = '\uF159';
        public const char DialUp = '\uE83C';
        public const char Dictionary = '\uE82D';
        public const char DictionaryAdd = '\uE82E';
        public const char DictionaryCloud = '\uEBC3';
        public const char DirectAccess = '\uE83B';
        public const char Directions = '\uE8F0';
        public const char DisableUpdates = '\uE8D8';
        public const char DisconnectDisplay = '\uEA14';
        public const char DisconnectDrive = '\uE8CD';
        public const char Dislike = '\uE8E0';
        public const char DMC = '\uE951';
        public const char Dock = '\uE952';
        public const char DockBottom = '\uE90E';
        public const char DockLeft = '\uE90C';
        public const char DockLeftMirrored = '\uEA4C';
        public const char DockRight = '\uE90D';
        public const char DockRightMirrored = '\uEA4B';
        public const char Document = '\uE8A5';
        public const char DoublePinyin = '\uF085';
        public const char Down = '\uE74B';
        public const char Download = '\uE896';
        public const char DownloadMap = '\uE826';
        public const char DownShiftKey = '\uE84A';
        public const char Dpad = '\uF10E';
        public const char Draw = '\uEC87';
        public const char DrawSolid = '\uEC88';
        public const char DrivingMode = '\uE7EC';
        public const char Drop = '\uEB42';
        public const char DullSound = '\uE911';
        public const char DullSoundKey = '\uE9AF';
        public const char Ear = '\uF270';
        public const char EaseOfAccess = '\uE776';
        public const char Edit = '\uE70F';
        public const char EditMirrored = '\uEB7E';
        public const char Education = '\uE7BE';
        public const char Emoji = '\uE899';
        public const char Emoji2 = '\uE76E';
        public const char EmojiSwatch = '\uED5B';
        public const char EmojiTabCelebrationObjects = '\uED55';
        public const char EmojiTabFavorites = '\uED5A';
        public const char EmojiTabFoodPlants = '\uED56';
        public const char EmojiTabPeople = '\uED53';
        public const char EmojiTabSmilesAnimals = '\uED54';
        public const char EmojiTabSymbols = '\uED58';
        public const char EmojiTabTextSmiles = '\uED59';
        public const char EmojiTabTransitPlaces = '\uED57';
        public const char EndPoint = '\uE81B';
        public const char EndPointSolid = '\uEB4B';
        public const char EnglishPunctuation = '\uF110';
        public const char EraseTool = '\uE75C';
        public const char EraseToolFill = '\uE82B';
        public const char EraseToolFill2 = '\uE82C';
        public const char Error = '\uE783';
        public const char ErrorBadge = '\uEA39';
        public const char ErrorBadge12 = '\uEDAE';
        public const char eSIM = '\uED2A';
        public const char eSIMBusy = '\uED2D';
        public const char eSIMLocked = '\uED2C';
        public const char eSIMNoProfile = '\uED2B';
        public const char Ethernet = '\uE839';
        public const char EthernetError = '\uEB55';
        public const char EthernetWarning = '\uEB56';
        public const char ExpandTile = '\uE976';
        public const char ExpandTileMirrored = '\uEA4E';
        public const char ExploitProtectionSettings = '\uF259';
        public const char ExploreContent = '\uECCD';
        public const char ExploreContentSingle = '\uF164';
        public const char Export = '\uEDE1';
        public const char ExportMirrored = '\uEDE2';
        public const char FastForward = '\uEB9D';
        public const char Favorite = '\uE113';
        public const char FavoriteList = '\uE728';
        public const char FavoriteStar = '\uE734';
        public const char FavoriteStarFill = '\uE735';
        public const char Feedback = '\uED15';
        public const char FeedbackApp = '\uE939';
        public const char Ferry = '\uE7E3';
        public const char FerrySolid = '\uEB48';
        public const char FileExplorer = '\uEC50';
        public const char FileExplorerApp = '\uEC51';
        public const char Filter = '\uE71C';
        public const char Find = '\uE11A';
        public const char FingerInking = '\uED5F';
        public const char Fingerprint = '\uE928';
        public const char Flag = '\uE7C1';
        public const char Flashlight = '\uE754';
        public const char FlickDown = '\uE935';
        public const char FlickLeft = '\uE937';
        public const char FlickRight = '\uE938';
        public const char FlickUp = '\uE936';
        public const char Folder = '\uE8B7';
        public const char FolderFill = '\uE8D5';
        public const char FolderHorizontal = '\uF12B';
        public const char Font = '\uE8D2';
        public const char FontColor = '\uE8D3';
        public const char FontDecrease = '\uE8E7';
        public const char FontIncrease = '\uE8E8';
        public const char FontSize = '\uE8E9';
        public const char Forward = '\uE72A';
        public const char ForwardMirrored = '\uF0D3';
        public const char ForwardSm = '\uE9AC';
        public const char FourBars = '\uE908';
        public const char Frigid = '\uE9CA';
        public const char FullAlpha = '\uE97F';
        public const char FullCircleMask = '\uE91F';
        public const char FullHiragana = '\uE986';
        public const char FullKatakana = '\uE987';
        public const char FullScreen = '\uE740';
        public const char Game = '\uE7FC';
        public const char GameConsole = '\uE967';
        public const char GlobalNavigationButton = '\uE700';
        public const char Globe = '\uE774';
        public const char Go = '\uE8AD';
        public const char GoMirrored = '\uEA4F';
        public const char GoToStart = '\uE8FC';
        public const char GotoToday = '\uE8D1';
        public const char GridView = '\uF0E2';
        public const char GripperBarHorizontal = '\uE76F';
        public const char GripperBarVertical = '\uE784';
        public const char GripperResize = '\uE788';
        public const char GripperResizeMirrored = '\uEA50';
        public const char GripperTool = '\uE75E';
        public const char Groceries = '\uEC09';
        public const char GroupList = '\uF168';
        public const char GuestUser = '\uEE57';
        public const char HalfAlpha = '\uE97E';
        public const char HalfDullSound = '\uE9B0';
        public const char HalfKatakana = '\uE988';
        public const char HalfStarLeft = '\uE7C6';
        public const char HalfStarRight = '\uE7C7';
        public const char Handwriting = '\uE929';
        public const char HangUp = '\uE778';
        public const char HardDrive = '\uEDA2';
        public const char HeadlessDevice = '\uF191';
        public const char Headphone = '\uE7F6';
        public const char Headphone0 = '\uED30';
        public const char Headphone1 = '\uED31';
        public const char Headphone2 = '\uED32';
        public const char Headphone3 = '\uED33';
        public const char Headset = '\uE95B';
        public const char Health = '\uE95E';
        public const char Heart = '\uEB51';
        public const char HeartBroken = '\uEA92';
        public const char HeartFill = '\uEB52';
        public const char Help = '\uE897';
        public const char HelpMirrored = '\uEA51';
        public const char HideBcc = '\uE8C5';
        public const char Highlight = '\uE7E6';
        public const char HighlightFill = '\uE891';
        public const char HighlightFill2 = '\uE82A';
        public const char History = '\uE81C';
        public const char HMD = '\uF119';
        public const char Home = '\uE80F';
        public const char HomeGroup = '\uEC26';
        public const char HomeSolid = '\uEA8A';
        public const char HorizontalTabKey = '\uE7FD';
        public const char IBeam = '\uE933';
        public const char IBeamOutline = '\uE934';
        public const char ImageExport = '\uEE71';
        public const char Import = '\uE8B5';
        public const char ImportAll = '\uE8B6';
        public const char ImportAllMirrored = '\uEA53';
        public const char Important = '\uE8C9';
        public const char ImportantBadge12 = '\uEDB1';
        public const char ImportMirrored = '\uEA52';
        public const char IncidentTriangle = '\uE814';
        public const char Info = '\uE946';
        public const char Info2 = '\uEA1F';
        public const char InfoSolid = '\uF167';
        public const char InkingCaret = '\uED65';
        public const char InkingColorFill = '\uED67';
        public const char InkingColorOutline = '\uED66';
        public const char InkingTool = '\uE76D';
        public const char InkingToolFill = '\uE88F';
        public const char InkingToolFill2 = '\uE829';
        public const char InPrivate = '\uE727';
        public const char Input = '\uE961';
        public const char InsiderHubApp = '\uEC24';
        public const char InternetSharing = '\uE704';
        public const char IOT = '\uF22C';
        public const char Italic = '\uE8DB';
        public const char Japanese = '\uE985';
        public const char JpnRomanji = '\uE87C';
        public const char JpnRomanjiLock = '\uE87D';
        public const char JpnRomanjiShift = '\uE87E';
        public const char JpnRomanjiShiftLock = '\uE87F';
        public const char Key12On = '\uE980';
        public const char Keyboard = '\uE144';
        public const char Keyboard12Key = '\uF261';
        public const char KeyboardBrightness = '\uED39';
        public const char KeyboardClassic = '\uE765';
        public const char KeyboardDismiss = '\uE92F';
        public const char KeyboardDock = '\uF26B';
        public const char KeyboardFull = '\uEC31';
        public const char KeyboardLeftAligned = '\uF20C';
        public const char KeyboardLeftDock = '\uF26D';
        public const char KeyboardLeftHanded = '\uE763';
        public const char KeyboardLowerBrightness = '\uED3A';
        public const char KeyboardNarrow = '\uF260';
        public const char KeyboardOneHanded = '\uED4C';
        public const char KeyboardRightAligned = '\uF20D';
        public const char KeyboardRightDock = '\uF26E';
        public const char KeyboardRightHanded = '\uE764';
        public const char KeyboardSettings = '\uF210';
        public const char KeyboardShortcut = '\uEDA7';
        public const char KeyboardSplit = '\uE766';
        public const char KeyboardStandard = '\uE92E';
        public const char KeyboardUndock = '\uF26C';
        public const char Korean = '\uE97D';
        public const char Label = '\uE932';
        public const char LangJPN = '\uE7DE';
        public const char LanguageChs = '\uE88D';
        public const char LanguageCht = '\uE88C';
        public const char LanguageJpn = '\uEC45';
        public const char LanguageKor = '\uE88B';
        public const char LaptopSelected = '\uEC76';
        public const char LargeErase = '\uF12A';
        public const char Leaf = '\uE8BE';
        public const char LeaveChat = '\uE89B';
        public const char LeaveChatMirrored = '\uEA54';
        public const char LEDLight = '\uE781';
        public const char LeftArrowKeyTime0 = '\uEC52';
        public const char LeftDoubleQuote = '\uE9B2';
        public const char LeftQuote = '\uE848';
        public const char LeftStick = '\uF108';
        public const char Lexicon = '\uF180';
        public const char Library = '\uE8F1';
        public const char Light = '\uE793';
        public const char Lightbulb = '\uEA80';
        public const char LightningBolt = '\uE945';
        public const char Like = '\uE8E1';
        public const char LikeDislike = '\uE8DF';
        public const char Link = '\uE71B';
        public const char List = '\uEA37';
        public const char ListMirrored = '\uEA55';
        public const char Location = '\uE81D';
        public const char Lock = '\uE72E';
        public const char LockscreenDesktop = '\uEE3F';
        public const char LockScreenGlance = '\uEE65';
        public const char LowerBrightness = '\uEC8A';
        public const char MagStripeReader = '\uEC5C';
        public const char Mail = '\uE715';
        public const char MailBadge12 = '\uEDB3';
        public const char MailFill = '\uE8A8';
        public const char MailFilled = '\uE135';
        public const char MailForward = '\uE89C';
        public const char MailForwardMirrored = '\uEA56';
        public const char MailReply = '\uE8CA';
        public const char MailReplyAll = '\uE8C2';
        public const char MailReplyAllMirrored = '\uEA58';
        public const char MailReplyMirrored = '\uEA57';
        public const char Manage = '\uE912';
        public const char Map = '\uE1C4';
        public const char MapCompassBottom = '\uE813';
        public const char MapCompassTop = '\uE812';
        public const char MapDirections = '\uE816';
        public const char MapDrive = '\uE8CE';
        public const char MapLayers = '\uE81E';
        public const char MapPin = '\uE707';
        public const char MapPin2 = '\uE7B7';
        public const char Marker = '\uED64';
        public const char Marquee = '\uEF20';
        public const char Media = '\uEA69';
        public const char MediaStorageTower = '\uE965';
        public const char Megaphone = '\uE789';
        public const char Memo = '\uE77C';
        public const char Message = '\uE8BD';
        public const char MicClipping = '\uEC72';
        public const char MicError = '\uEC56';
        public const char MicOff = '\uEC54';
        public const char MicOn = '\uEC71';
        public const char Microphone = '\uE720';
        public const char MicrophoneListening = '\uF12E';
        public const char MicSleep = '\uEC55';
        public const char MiracastLogoLarge = '\uEC16';
        public const char MiracastLogoSmall = '\uEC15';
        public const char MobActionCenter = '\uEC42';
        public const char MobAirplane = '\uEC40';
        public const char MobBattery0 = '\uEBA0';
        public const char MobBattery1 = '\uEBA1';
        public const char MobBattery10 = '\uEBAA';
        public const char MobBattery2 = '\uEBA2';
        public const char MobBattery3 = '\uEBA3';
        public const char MobBattery4 = '\uEBA4';
        public const char MobBattery5 = '\uEBA5';
        public const char MobBattery6 = '\uEBA6';
        public const char MobBattery7 = '\uEBA7';
        public const char MobBattery8 = '\uEBA8';
        public const char MobBattery9 = '\uEBA9';
        public const char MobBatteryCharging0 = '\uEBAB';
        public const char MobBatteryCharging1 = '\uEBAC';
        public const char MobBatteryCharging10 = '\uEBB5';
        public const char MobBatteryCharging2 = '\uEBAD';
        public const char MobBatteryCharging3 = '\uEBAE';
        public const char MobBatteryCharging4 = '\uEBAF';
        public const char MobBatteryCharging5 = '\uEBB0';
        public const char MobBatteryCharging6 = '\uEBB1';
        public const char MobBatteryCharging7 = '\uEBB2';
        public const char MobBatteryCharging8 = '\uEBB3';
        public const char MobBatteryCharging9 = '\uEBB4';
        public const char MobBatterySaver0 = '\uEBB6';
        public const char MobBatterySaver1 = '\uEBB7';
        public const char MobBatterySaver10 = '\uEBC0';
        public const char MobBatterySaver2 = '\uEBB8';
        public const char MobBatterySaver3 = '\uEBB9';
        public const char MobBatterySaver4 = '\uEBBA';
        public const char MobBatterySaver5 = '\uEBBB';
        public const char MobBatterySaver6 = '\uEBBC';
        public const char MobBatterySaver7 = '\uEBBD';
        public const char MobBatterySaver8 = '\uEBBE';
        public const char MobBatterySaver9 = '\uEBBF';
        public const char MobBatteryUnknown = '\uEC02';
        public const char MobBluetooth = '\uEC41';
        public const char MobCallForwarding = '\uEC7E';
        public const char MobCallForwardingMirrored = '\uEC7F';
        public const char MobDrivingMode = '\uEC47';
        public const char MobileLocked = '\uEC20';
        public const char MobileSelected = '\uEC75';
        public const char MobileTablet = '\uE8CC';
        public const char MobLocation = '\uEC43';
        public const char MobQuietHours = '\uEC46';
        public const char MobSignal1 = '\uEC37';
        public const char MobSignal2 = '\uEC38';
        public const char MobSignal3 = '\uEC39';
        public const char MobSignal4 = '\uEC3A';
        public const char MobSignal5 = '\uEC3B';
        public const char MobWifi1 = '\uEC3C';
        public const char MobWifi2 = '\uEC3D';
        public const char MobWifi3 = '\uEC3E';
        public const char MobWifi4 = '\uEC3F';
        public const char MobWifiHotspot = '\uEC44';
        public const char More = '\uE712';
        public const char Mouse = '\uE962';
        public const char MoveToFolder = '\uE8DE';
        public const char Movies = '\uE8B2';
        public const char MultimediaDMP = '\uED47';
        public const char MultimediaDMS = '\uE953';
        public const char MultimediaDVR = '\uE954';
        public const char MultimediaPMP = '\uE955';
        public const char MultiSelect = '\uE762';
        public const char MultiSelectMirrored = '\uEA98';
        public const char Multitask = '\uE7C4';
        public const char Multitask16 = '\uEE40';
        public const char MultitaskExpanded = '\uEB91';
        public const char MusicAlbum = '\uE93C';
        public const char MusicInfo = '\uE90B';
        public const char MusicNote = '\uEC4F';
        public const char Mute = '\uE74F';
        public const char MyNetwork = '\uEC27';
        public const char Narrator = '\uED4D';
        public const char NarratorForward = '\uEDA9';
        public const char NarratorForwardMirrored = '\uEDAA';
        public const char Network = '\uE968';
        public const char NetworkAdapter = '\uEDA3';
        public const char NetworkConnected = '\uF385';
        public const char NetworkConnectedCheckmark = '\uF386';
        public const char NetworkOffline = '\uF384';
        public const char NetworkPrinter = '\uEDA5';
        public const char NetworkSharing = '\uF193';
        public const char NetworkTower = '\uEC05';
        public const char NewFolder = '\uE8F4';
        public const char NewWindow = '\uE78B';
        public const char Next = '\uE893';
        public const char NUIFace = '\uEB68';
        public const char NUIFPContinueSlideAction = '\uEB85';
        public const char NUIFPContinueSlideHand = '\uEB84';
        public const char NUIFPPressAction = '\uEB8B';
        public const char NUIFPPressHand = '\uEB8A';
        public const char NUIFPPressRepeatAction = '\uEB8D';
        public const char NUIFPPressRepeatHand = '\uEB8C';
        public const char NUIFPRollLeftAction = '\uEB89';
        public const char NUIFPRollLeftHand = '\uEB88';
        public const char NUIFPRollRightHand = '\uEB86';
        public const char NUIFPRollRightHandAction = '\uEB87';
        public const char NUIFPStartSlideAction = '\uEB83';
        public const char NUIFPStartSlideHand = '\uEB82';
        public const char NUIIris = '\uEB67';
        public const char OEM = '\uE74C';
        public const char OneBar = '\uE905';
        public const char OpenFile = '\uE8E5';
        public const char OpenInNewWindow = '\uE8A7';
        public const char OpenLocal = '\uE8DA';
        public const char OpenPane = '\uE8A0';
        public const char OpenPaneMirrored = '\uEA5B';
        public const char OpenWith = '\uE7AC';
        public const char OpenWithMirrored = '\uEA5C';
        public const char Orientation = '\uE8B4';
        public const char OtherUser = '\uE7EE';
        public const char OutlineHalfStarLeft = '\uF0E7';
        public const char OutlineHalfStarRight = '\uF0E8';
        public const char OutlineQuarterStarLeft = '\uF0E5';
        public const char OutlineQuarterStarRight = '\uF0E6';
        public const char OutlineStar = '\uE1CE';
        public const char OutlineStarLeftHalf = '\uF0F7';
        public const char OutlineStarRightHalf = '\uF0F8';
        public const char OutlineThreeQuarterStarLeft = '\uF0E9';
        public const char OutlineThreeQuarterStarRight = '\uF0EA';
        public const char Package = '\uE7B8';
        public const char Page = '\uE7C3';
        public const char Page2 = '\uE160';
        public const char PageLeft = '\uE760';
        public const char PageRight = '\uE761';
        public const char PageSolid = '\uE729';
        public const char PaginationDotOutline10 = '\uF126';
        public const char PaginationDotSolid10 = '\uF127';
        public const char PanMode = '\uECE9';
        public const char ParkingLocation = '\uE811';
        public const char ParkingLocationMirrored = '\uEA5E';
        public const char ParkingLocationSolid = '\uEA8B';
        public const char PartyLeader = '\uECA7';
        public const char PassiveAuthentication = '\uF32A';
        public const char PasswordKeyHide = '\uE9A9';
        public const char PasswordKeyShow = '\uE9A8';
        public const char Paste = '\uE77F';
        public const char Pause = '\uE769';
        public const char PauseBadge12 = '\uEDB4';
        public const char PC1 = '\uE977';
        public const char Pencil = '\uED63';
        public const char PencilFill = '\uF0C6';
        public const char PenPalette = '\uEE56';
        public const char PenPaletteMirrored = '\uEF16';
        public const char PenWorkspace = '\uEDC6';
        public const char PenWorkspaceMirrored = '\uEF15';
        public const char People = '\uE716';
        public const char PeriodKey = '\uE843';
        public const char Permissions = '\uE8D7';
        public const char PersonalFolder = '\uEC25';
        public const char Personalize = '\uE771';
        public const char Phone = '\uE717';
        public const char PhoneBook = '\uE780';
        public const char Photo = '\uE91B';
        public const char Photo2 = '\uEB9F';
        public const char Picture = '\uE8B9';
        public const char Pictures = '\uE158';
        public const char PieSingle = '\uEB05';
        public const char Pin = '\uE718';
        public const char PinFill = '\uE841';
        public const char Pinned = '\uE840';
        public const char PinnedFill = '\uE842';
        public const char Placeholder = '\uE18A';
        public const char PLAP = '\uEC19';
        public const char Play = '\uE768';
        public const char Play36 = '\uEE4A';
        public const char PlaybackRate1x = '\uEC57';
        public const char PlaybackRateOther = '\uEC58';
        public const char PlayBadge12 = '\uEDB5';
        public const char PointErase = '\uED61';
        public const char PointEraseMirrored = '\uEF18';
        public const char PointerHand = '\uF271';
        public const char PoliceCar = '\uEC81';
        public const char PostUpdate = '\uE8F3';
        public const char PowerButton = '\uE7E8';
        public const char PresenceChicklet = '\uE978';
        public const char PresenceChickletVideo = '\uE979';
        public const char Preview = '\uE8FF';
        public const char PreviewLink = '\uE8A1';
        public const char Previous = '\uE892';
        public const char Print = '\uE749';
        public const char Printer3D = '\uE914';
        public const char PrintfaxPrinterFile = '\uE956';
        public const char Priority = '\uE8D0';
        public const char Process = '\uE9F3';
        public const char ProgressRingDots = '\uF16A';
        public const char Project = '\uEBC6';
        public const char Projector = '\uE95D';
        public const char ProtectedDocument = '\uE8A6';
        public const char Protractor = '\uF0B4';
        public const char ProvisioningPackage = '\uE835';
        public const char PuncKey = '\uE844';
        public const char PuncKey0 = '\uE84C';
        public const char PuncKey1 = '\uE9B4';
        public const char PuncKey2 = '\uE9B5';
        public const char PuncKey3 = '\uE9B6';
        public const char PuncKey4 = '\uE9B7';
        public const char PuncKey5 = '\uE9B8';
        public const char PuncKey6 = '\uE9B9';
        public const char PuncKey7 = '\uE9BB';
        public const char PuncKey8 = '\uE9BC';
        public const char PuncKey9 = '\uE9BA';
        public const char PuncKeyLeftBottom = '\uE84D';
        public const char PuncKeyRightBottom = '\uE9B3';
        public const char Puzzle = '\uEA86';
        public const char QuarentinedItems = '\uF0B2';
        public const char QuarentinedItemsMirrored = '\uF0B3';
        public const char QuarterStarLeft = '\uF0CA';
        public const char QuarterStarRight = '\uF0CB';
        public const char QuickNote = '\uE70B';
        public const char QuietHours = '\uE708';
        public const char QuietHoursBadge12 = '\uF0CE';
        public const char QWERTYOff = '\uE983';
        public const char QWERTYOn = '\uE982';
        public const char RadioBtnOff = '\uECCA';
        public const char RadioBtnOn = '\uECCB';
        public const char RadioBullet = '\uE915';
        public const char RadioBullet2 = '\uECCC';
        public const char Read = '\uE8C3';
        public const char ReadingList = '\uE7BC';
        public const char ReceiptPrinter = '\uEC5B';
        public const char Recent = '\uE823';
        public const char Record = '\uE7C8';
        public const char Redo = '\uE7A6';
        public const char Refresh = '\uE72C';
        public const char Relationship = '\uF003';
        public const char RememberedDevice = '\uE70C';
        public const char Reminder = '\uEB50';
        public const char ReminderFill = '\uEB4F';
        public const char Remote = '\uE8AF';
        public const char Remove = '\uE738';
        public const char RemoveFrom = '\uECC9';
        public const char Rename = '\uE8AC';
        public const char Repair = '\uE90F';
        public const char RepeatAll = '\uE8EE';
        public const char RepeatOne = '\uE8ED';
        public const char Reply = '\uE97A';
        public const char ReplyMirrored = '\uEE35';
        public const char ReportHacked = '\uE730';
        public const char ResetDevice = '\uED10';
        public const char ResetDrive = '\uEBC4';
        public const char Reshare = '\uE8EB';
        public const char ResizeMouseLarge = '\uE747';
        public const char ResizeMouseMedium = '\uE744';
        public const char ResizeMouseMediumMirrored = '\uEA5F';
        public const char ResizeMouseSmall = '\uE743';
        public const char ResizeMouseSmallMirrored = '\uEA60';
        public const char ResizeMouseTall = '\uE746';
        public const char ResizeMouseTallMirrored = '\uEA61';
        public const char ResizeMouseWide = '\uE745';
        public const char ResizeTouchLarger = '\uE741';
        public const char ResizeTouchNarrower = '\uE7EA';
        public const char ResizeTouchNarrowerMirrored = '\uEA62';
        public const char ResizeTouchShorter = '\uE7EB';
        public const char ResizeTouchSmaller = '\uE742';
        public const char ReturnKey = '\uE751';
        public const char ReturnKeyLg = '\uEB97';
        public const char ReturnKeySm = '\uE966';
        public const char ReturnToWindow = '\uE944';
        public const char RevToggleKey = '\uE845';
        public const char Rewind = '\uEB9E';
        public const char RightArrowKeyTime0 = '\uEBE7';
        public const char RightArrowKeyTime1 = '\uE846';
        public const char RightArrowKeyTime2 = '\uE847';
        public const char RightArrowKeyTime3 = '\uE84E';
        public const char RightArrowKeyTime4 = '\uE84F';
        public const char RightDoubleQuote = '\uE9B1';
        public const char RightQuote = '\uE849';
        public const char RightStick = '\uF109';
        public const char Ringer = '\uEA8F';
        public const char RingerBadge12 = '\uEDAC';
        public const char RingerSilent = '\uE7ED';
        public const char RoamingDomestic = '\uE879';
        public const char RoamingInternational = '\uE878';
        public const char Robot = '\uE99A';
        public const char Rotate = '\uE7AD';
        public const char RotateCamera = '\uE89E';
        public const char RotateMapLeft = '\uE80D';
        public const char RotateMapRight = '\uE80C';
        public const char RotationLock = '\uE755';
        public const char Ruler = '\uED5E';
        public const char Save = '\uE74E';
        public const char SaveAs = '\uE792';
        public const char SaveCopy = '\uEA35';
        public const char SaveLocal = '\uE78C';
        public const char Scan = '\uE8FE';
        public const char ScreenTime = '\uF182';
        public const char ScrollMode = '\uECE7';
        public const char ScrollUpDown = '\uEC8F';
        public const char SDCard = '\uE7F1';
        public const char Search = '\uE721';
        public const char SearchAndApps = '\uE773';
        public const char SelectAll = '\uE8B3';
        public const char Send = '\uE724';
        public const char SendFill = '\uE725';
        public const char SendFillMirrored = '\uEA64';
        public const char SendMirrored = '\uEA63';
        public const char Sensor = '\uE957';
        public const char SetlockScreen = '\uE7B5';
        public const char SetTile = '\uE97B';
        public const char Setting = '\uE115';
        public const char Settings = '\uE713';
        public const char SettingsBattery = '\uEE63';
        public const char SettingsDisplaySound = '\uE7F3';
        public const char Share = '\uE72D';
        public const char ShareBroadband = '\uE83A';
        public const char Shop = '\uE719';
        public const char ShoppingCart = '\uE7BF';
        public const char ShowBcc = '\uE8C4';
        public const char ShowResults = '\uE8BC';
        public const char ShowResultsMirrored = '\uEA65';
        public const char Shuffle = '\uE8B1';
        public const char SignalBars1 = '\uE86C';
        public const char SignalBars2 = '\uE86D';
        public const char SignalBars3 = '\uE86E';
        public const char SignalBars4 = '\uE86F';
        public const char SignalBars5 = '\uE870';
        public const char SignalError = '\uED2E';
        public const char SignalNotConnected = '\uE871';
        public const char SignalRoaming = '\uEC1E';
        public const char SIMLock = '\uE875';
        public const char SIMMissing = '\uE876';
        public const char SIPMove = '\uE759';
        public const char SIPRedock = '\uE75B';
        public const char SIPUndock = '\uE75A';
        public const char SkipBack10 = '\uED3C';
        public const char SkipForward30 = '\uED3D';
        public const char SliderThumb = '\uEC13';
        public const char Slideshow = '\uE786';
        public const char SmallErase = '\uF129';
        public const char Smartcard = '\uE963';
        public const char SmartcardVirtual = '\uE964';
        public const char SolidStar = '\uE1CF';
        public const char Sort = '\uE8CB';
        public const char SpatialVolume0 = '\uF0EB';
        public const char SpatialVolume1 = '\uF0EC';
        public const char SpatialVolume2 = '\uF0ED';
        public const char SpatialVolume3 = '\uF0EE';
        public const char Speakers = '\uE7F5';
        public const char SpeedHigh = '\uEC4A';
        public const char SpeedMedium = '\uEC49';
        public const char SpeedOff = '\uEC48';
        public const char StartPoint = '\uE819';
        public const char StartPointSolid = '\uEB49';
        public const char StatusCheckmark = '\uF1D8';
        public const char StatusCheckmark7 = '\uF0B7';
        public const char StatusCheckmarkLeft = '\uF1D9';
        public const char StatusCircle = '\uEA81';
        public const char StatusCircle7 = '\uF0B6';
        public const char StatusCircleBlock = '\uF140';
        public const char StatusCircleBlock2 = '\uF141';
        public const char StatusCircleCheckmark = '\uF13E';
        public const char StatusCircleErrorX = '\uF13D';
        public const char StatusCircleExclamation = '\uF13C';
        public const char StatusCircleInfo = '\uF13F';
        public const char StatusCircleInner = '\uF137';
        public const char StatusCircleLeft = '\uEBFD';
        public const char StatusCircleOuter = '\uF136';
        public const char StatusCircleQuestionMark = '\uF142';
        public const char StatusCircleRing = '\uF138';
        public const char StatusCircleSync = '\uF143';
        public const char StatusConnecting1 = '\uEB57';
        public const char StatusConnecting2 = '\uEB58';
        public const char StatusDataTransfer = '\uE880';
        public const char StatusDataTransferVPN = '\uE881';
        public const char StatusDualSIM1 = '\uE884';
        public const char StatusDualSIM1VPN = '\uE885';
        public const char StatusDualSIM2 = '\uE882';
        public const char StatusDualSIM2VPN = '\uE883';
        public const char StatusError = '\uEA83';
        public const char StatusErrorCircle7 = '\uF0B8';
        public const char StatusErrorFull = '\uEB90';
        public const char StatusErrorLeft = '\uEBFF';
        public const char StatusExclamationCircle7 = '\uF12F';
        public const char StatusPause7 = '\uF175';
        public const char StatusSGLTE = '\uE886';
        public const char StatusSGLTECell = '\uE887';
        public const char StatusSGLTEDataVPN = '\uE888';
        public const char StatusTriangle = '\uEA82';
        public const char StatusTriangleExclamation = '\uF13B';
        public const char StatusTriangleInner = '\uF13A';
        public const char StatusTriangleLeft = '\uEBFE';
        public const char StatusTriangleOuter = '\uF139';
        public const char StatusUnsecure = '\uEB59';
        public const char StatusVPN = '\uE889';
        public const char StatusWarning = '\uEA84';
        public const char StatusWarningLeft = '\uEC00';
        public const char StockDown = '\uEB0F';
        public const char StockUp = '\uEB11';
        public const char Stop = '\uE71A';
        public const char StopPoint = '\uE81A';
        public const char StopPointSolid = '\uEB4A';
        public const char StopSlideShow = '\uE191';
        public const char Stopwatch = '\uE916';
        public const char StorageNetworkWireless = '\uE969';
        public const char StorageOptical = '\uE958';
        public const char StorageTape = '\uE96A';
        public const char Streaming = '\uE93E';
        public const char StreamingEnterprise = '\uED2F';
        public const char Street = '\uE913';
        public const char StreetsideSplitExpand = '\uE803';
        public const char StreetsideSplitMinimize = '\uE802';
        public const char StrokeErase = '\uED60';
        public const char StrokeErase2 = '\uF128';
        public const char StrokeEraseMirrored = '\uEF17';
        public const char Subtitles = '\uED1E';
        public const char SubtitlesAudio = '\uED1F';
        public const char SurfaceHub = '\uE8AE';
        public const char Sustainable = '\uEC0A';
        public const char Swipe = '\uE927';
        public const char SwipeRevealArt = '\uEC6D';
        public const char Switch = '\uE8AB';
        public const char SwitchApps = '\uE8F9';
        public const char SwitchUser = '\uE748';
        public const char Sync = '\uE895';
        public const char SyncBadge12 = '\uEDAB';
        public const char SyncError = '\uEA6A';
        public const char SyncFolder = '\uE8F7';
        public const char System = '\uE770';
        public const char Tablet = '\uE70A';
        public const char TabletMode = '\uEBFC';
        public const char TabletSelected = '\uEC74';
        public const char Tag = '\uE8EC';
        public const char TapAndSend = '\uE9A1';
        public const char Target = '\uE1D2';
        public const char TaskbarPhone = '\uEE64';
        public const char ThisPC = '\uEC4E';
        public const char ThoughtBubble = '\uEA91';
        public const char ThreeBars = '\uE907';
        public const char ThreeQuarterStarLeft = '\uF0CC';
        public const char ThreeQuarterStarRight = '\uF0CD';
        public const char Tiles = '\uECA5';
        public const char TiltDown = '\uE80A';
        public const char TiltUp = '\uE809';
        public const char TimeLanguage = '\uE775';
        public const char ToggleBorder = '\uEC12';
        public const char ToggleFilled = '\uEC11';
        public const char ToggleThumb = '\uEC14';
        public const char TollSolid = '\uF161';
        public const char ToolTip = '\uE82F';
        public const char Touch = '\uE815';
        public const char TouchPointer = '\uE7C9';
        public const char Touchscreen = '\uEDA4';
        public const char Trackers = '\uEADF';
        public const char TrackersMirrored = '\uEE92';
        public const char TrafficCongestionSolid = '\uF163';
        public const char Train = '\uE7C0';
        public const char TrainSolid = '\uEB4D';
        public const char TreeFolderFolder = '\uED41';
        public const char TreeFolderFolderFill = '\uED42';
        public const char TreeFolderFolderOpen = '\uED43';
        public const char TreeFolderFolderOpenFill = '\uED44';
        public const char TriggerLeft = '\uF10A';
        public const char TriggerRight = '\uF10B';
        public const char Trim = '\uE78A';
        public const char TVMonitor = '\uE7F4';
        public const char TVMonitorSelected = '\uEC77';
        public const char TwoBars = '\uE906';
        public const char TwoPage = '\uE89A';
        public const char Type = '\uE97C';
        public const char Underline = '\uE8DC';
        public const char UnderscoreSpace = '\uE75D';
        public const char Undo = '\uE7A7';
        public const char Unfavorite = '\uE8D9';
        public const char Unit = '\uECC6';
        public const char Unlock = '\uE785';
        public const char Unpin = '\uE77A';
        public const char UnsyncFolder = '\uE8F6';
        public const char Up = '\uE74A';
        public const char UpArrowShiftKey = '\uE752';
        public const char UpdateRestore = '\uE777';
        public const char Upload = '\uE898';
        public const char UpShiftKey = '\uE84B';
        public const char USB = '\uE88E';
        public const char USBSafeConnect = '\uECF3';
        public const char UserAPN = '\uF081';
        public const char Vibrate = '\uE877';
        public const char Video = '\uE714';
        public const char VideoChat = '\uE8AA';
        public const char View = '\uE890';
        public const char ViewAll = '\uE8A9';
        public const char Volume = '\uE767';
        public const char Volume0 = '\uE992';
        public const char Volume1 = '\uE993';
        public const char Volume2 = '\uE994';
        public const char Volume3 = '\uE995';
        public const char VolumeBars = '\uEBC5';
        public const char VPN = '\uE705';
        public const char Walk = '\uE805';
        public const char WalkSolid = '\uE726';
        public const char Warning = '\uE7BA';
        public const char Webcam = '\uE8B8';
        public const char Webcam2 = '\uE960';
        public const char Wheel = '\uEE94';
        public const char Wifi = '\uE701';
        public const char Wifi1 = '\uE872';
        public const char Wifi2 = '\uE873';
        public const char Wifi3 = '\uE874';
        public const char WifiAttentionOverlay = '\uE998';
        public const char WifiCall0 = '\uEBD5';
        public const char WifiCall1 = '\uEBD6';
        public const char WifiCall2 = '\uEBD7';
        public const char WifiCall3 = '\uEBD8';
        public const char WifiCall4 = '\uEBD9';
        public const char WifiCallBars = '\uEBD4';
        public const char WifiError0 = '\uEB5A';
        public const char WifiError1 = '\uEB5B';
        public const char WifiError2 = '\uEB5C';
        public const char WifiError3 = '\uEB5D';
        public const char WifiError4 = '\uEB5E';
        public const char WifiEthernet = '\uEE77';
        public const char WifiHotspot = '\uE88A';
        public const char WifiWarning0 = '\uEB5F';
        public const char WifiWarning1 = '\uEB60';
        public const char WifiWarning2 = '\uEB61';
        public const char WifiWarning3 = '\uEB62';
        public const char WifiWarning4 = '\uEB63';
        public const char WindDirection = '\uEBE6';
        public const char WindowsInsider = '\uF1AD';
        public const char WiredUSB = '\uECF0';
        public const char WirelessUSB = '\uECF1';
        public const char Work = '\uE821';
        public const char WorkSolid = '\uEB4E';
        public const char World = '\uE909';
        public const char XboxOneConsole = '\uE990';
        public const char ZeroBars = '\uE904';
        public const char Zoom = '\uE71E';
        public const char ZoomIn = '\uE8A3';
        public const char ZoomMode = '\uECE8';
        public const char ZoomOut = '\uE71F';

        #endregion




        //------------------------------------------------------
        //
        //  Additional Values
        //
        //------------------------------------------------------

        /* 
         * These values are not listed on the source WebPage but 
         * have valid Unicode entries in the MDL2 font. 
         */

        public const char AddUser = '\uE1E2';
        public const char BlockUser = '\uE1E0';
        public const char UserSettings = '\uEF58';
        public const char MixedRealityHeadset = '\uF119';

        public const char XboxButtonMenu = '\uEDE3';
        public const char XboxButtonView = '\uEECA';
        public const char XboxButtonA = '\uF093';
        public const char XboxButtonB = '\uF094';
        public const char XboxButtonY = '\uF095';
        public const char XboxButtonX = '\uF096';
        public const char XboxButtonLeftStick = '\uF108';
        public const char XboxButtonRightStick = '\uF109';
        public const char XboxButtonLeftTrigger = '\uF10A';
        public const char XboxButtonRightTrigger = '\uF10B';
        public const char XboxButtonLeftBumper = '\uF10C';
        public const char XboxButtonRightBumper = '\uF10D';
        public const char XboxButtonDPad = '\uF10E';





        //------------------------------------------------------
        //
        //  Markup Extension Implementation
        //
        //------------------------------------------------------

        public string Name { get; set; }

        public Symbol Symbol { get; set; }

        protected override object ProvideValue()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var prop = typeof(MDL2).GetField(Name);
                if (prop != null)
                {
                    // We must return string as returning char explodes
                    // the universe.
                    return prop.GetValue(this).ToString();
                }
            }
            else
            {
                return char.ConvertFromUtf32((int)Symbol);
            }

            return base.ProvideValue();
        }

        public override string ToString()
        {
            return ProvideValue().ToString();
        }




        //------------------------------------------------------
        //
        //  Reference Documentation
        //
        //------------------------------------------------------

        /*
         * Example Parsing Code.
         * This takes in an XML taken from the above sites source that looks like:
         * 
             * <xml>

                    <tr>
                      <td>
                        <img src="images/segoe-mdl/e700.png" alt="GlobalNavButton" data-linktype="relative-path"/>
                      </td>
                      <td>E700</td>
                      <td>GlobalNavigationButton</td>
                    </tr>
                    <tr>
                      <td>
                        <img src="images/segoe-mdl/e701.png" alt="Wifi" data-linktype="relative-path"/>
                      </td>
                      <td>E701</td>
                      <td>Wifi</td>
                    </tr>

              .....

              </xml>
        
                
            public async Task<String> ParseAsync()
            {
                var file = await Package.Current.InstalledLocation.GetFileAsync("Data.xml");
                var xml = await XmlDocument.LoadFromFileAsync(file);

                List<(string code, string name)> datas =
                    xml.ChildNodes[0].ChildNodes.Where(n => n.NodeType == NodeType.ElementNode && n.NodeName.Equals("tr")).Select(child =>
                    {
                        var trs = child.ChildNodes.Where(n => n.NodeType == NodeType.ElementNode && n.NodeName.Equals("td")).ToList();
                        return (trs[1].InnerText, trs[2].InnerText);
                    }).ToList();

                // Add any missing types from XAML symbol enum
                var type = typeof(Symbol);
                foreach (var e in Enum.GetValues(type))
                {
                    var name = Enum.GetName(type, e);
                    if (!datas.Any(d => d.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                        datas.Add((((int)e).ToString("X"), name));
                }

                StringBuilder builder = new StringBuilder();
                foreach (var (code, name) in datas.OrderBy(d => d.name))
                    builder.AppendLine($"public const char {name} = \'\\u{code}\';");

                return builder.ToString() ;
            }


         */
    }
}

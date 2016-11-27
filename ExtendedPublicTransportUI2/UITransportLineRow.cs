using ColossalFramework.UI;
using System;
using ColossalFramework;
using UnityEngine;

namespace EPTUI2
{
    public class UITransportLineRow : UIPanel
    {
        public delegate void LineNameChangedHandler(ushort lineID);
        public event LineNameChangedHandler LineNameChanged;

        private UICustomCheckbox _checkBox;
        private UIPanel _colorFieldPanel;
        private UIColorField _color;

        private UILabel _name;
        private UILabel _stops;
        private UILabel _passengers;
        private UILabel _trips;
        private UILabel _vehicles;
        private UICustomCheckbox _dayOnly;
        private UICustomCheckbox _nightOnly;

        private UIButton _deleteButton;

        private ushort _lineID;
        private InstanceID _instanceID;
        private string _lineName;

        public ushort LineID
        {
            get { return _lineID; }
            set
            {
                _lineID = value;
                _instanceID = new InstanceID { TransportLine = _lineID };
            }
        }

        public string LineName
        {
            get
            {
                if (_lineName == null)
                    return TransportUtil.GetLineName(LineID);
                return _lineName;
            }
            private set { _lineName = value; }
        }

        public uint Stops
        {
            get { return TransportUtil.GetStopCount (LineID); }
        }

        public uint TotalPassengers
        {
            get { return (TransportUtil.GetResidentPassengerCount(LineID) + TransportUtil.GetTouristPassengerCount(LineID)); }
        }

        public uint Trips 
        {
            get { return TransportUtil.GetTripsSaved (LineID); }
        }

        public uint Vehicles 
        {
            get { return TransportUtil.GetVehicleCount (LineID); }
        }
                
        public InstanceID InstanceID
        {
            get { return _instanceID; }
        }

        public bool IsOdd { get; set; }

        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        public bool IsChecked
        {
            get { return _checkBox.IsChecked; }
            set { _checkBox.IsChecked = value; }
        }

        public override void Awake()
        {
            base.Awake();

            height = 16;
            width = 535;//450;
        }

        public override void Start()
        {
            base.Start();

            _checkBox = AddUIComponent<UICustomCheckbox>();

            _colorFieldPanel = AddUIComponent<UIPanel>();
            _colorFieldPanel.size = new Vector2(17, 17);
            _colorFieldPanel.relativePosition = new Vector3(22, 0);

            _color = Instantiate(FindObjectOfType<UIColorField>().gameObject).GetComponent<UIColorField>();
            _colorFieldPanel.AttachUIComponent(_color.gameObject);
            _color.name = "ColorPickerLine" + LineID;
            _color.size = new Vector2(17, 17);
            _color.relativePosition = new Vector3(0, 0);
            _color.pickerPosition = UIColorField.ColorPickerPosition.RightAbove;
            _color.eventSelectedColorChanged += (component, value) => TransportUtil.SetLineColor(LineID, value);

            _name = AddUIComponent<UILabel>();
            _stops = AddUIComponent<UILabel>();
            _passengers = AddUIComponent<UILabel>();
            _trips = AddUIComponent<UILabel>();
            _vehicles = AddUIComponent<UILabel>();
            _dayOnly = AddUIComponent<UICustomCheckbox>();
            _nightOnly = AddUIComponent<UICustomCheckbox>();

            _deleteButton = AddUIComponent<UIButton>();
            _deleteButton.size = new Vector2(17, 17);
            _deleteButton.tooltip = "Delete line";
            _deleteButton.normalFgSprite = "buttonclose";
            _deleteButton.pressedFgSprite = "buttonclosepressed";
            _deleteButton.hoveredFgSprite = "buttonclosehover";
            _deleteButton.isVisible = true;

            _checkBox.relativePosition = new Vector3(5, 0);
            _name.relativePosition = new Vector3(43, 0);
            _stops.relativePosition = new Vector3(170, 0);
            _passengers.relativePosition = new Vector3(225, 0);
            _trips.relativePosition = new Vector3(320, 0);
            _vehicles.relativePosition = new Vector3(401, 0);
            _dayOnly.relativePosition = new Vector3(455, 0);
            _nightOnly.relativePosition = new Vector3(490, 0);
            _deleteButton.relativePosition = new Vector3(515, 0);

            _name.textColor = new Color32(185, 221, 254, 255);
            _stops.textColor = new Color32(185, 221, 254, 255);
            _passengers.textColor = new Color32(185, 221, 254, 255);
            _trips.textColor = new Color32(185, 221, 254, 255);
            _vehicles.textColor = new Color32(185, 221, 254, 255);

            _checkBox.size = new Vector2(12, 12);
            _dayOnly.size = new Vector2(12, 12);
            _nightOnly.size = new Vector2(12, 12);

            //init the day/night checkboxes
            _dayOnly.IsChecked = TransportUtil.GetDay(LineID);
            _nightOnly.IsChecked = TransportUtil.GetNight(LineID);

            // event handler for on/off of the line
            _checkBox.eventClick += (component, param) =>
            {
                _checkBox.IsChecked = !_checkBox.IsChecked;

                if (!_checkBox.IsChecked)
                    TransportUtil.HideTransportLine(LineID);
                else
                    TransportUtil.ShowTransportLine(LineID);
            };

            // event handler for on/off of day
            _dayOnly.eventClick += (component, param) =>
            {
                _dayOnly.IsChecked = !_dayOnly.IsChecked;
                                
                TransportUtil.SetDay(LineID, _dayOnly.IsChecked);            
            };

            // event handler for on/off of night
            _nightOnly.eventClick += (component, param) =>
            {
                _nightOnly.IsChecked = !_nightOnly.IsChecked;

                TransportUtil.SetNight(LineID, _nightOnly.IsChecked);
            };

            _name.eventClick += (component, param) =>
                WorldInfoPanel.Show<PublicTransportWorldInfoPanel>(TransportUtil.GetFirstLineStop(LineID), InstanceID);

            _name.eventMouseHover += (component, param) =>
            {
                TransportUtil.SelectTransportLine(LineID);
                _name.textColor = TransportUtil.GetLineColor(LineID);
            };

            _name.eventMouseLeave += (component, param) =>
            {
                TransportUtil.DeselectTransportLine(LineID);
                _name.textColor = new Color32(185, 221, 254, 255);
            };

            _deleteButton.eventClick += (component, param) =>
                Singleton<TransportManager>.instance.ReleaseLine(LineID);

            // scale label texts
            _name.textScale = 0.8f;
            _stops.textScale = 0.8f;
            _passengers.textScale = 0.8f;
            _trips.textScale = 0.8f;
            _vehicles.textScale = 0.8f;
            _deleteButton.textScale = 0.8f;

            // zebra stripes background
            backgroundSprite = "GenericPanelLight";
            if (IsOdd)
                color = new Color32(150, 150, 150, 255);
            else
                color = new Color32(130, 130, 130, 255);

            // center elements in row
            UIComponent[] children = GetComponentsInChildren<UIComponent>();
            foreach (UIComponent child in children)
            {
                if (child == this) continue;

                child.pivot = UIPivotPoint.MiddleLeft;
                child.transformPosition = new Vector3(child.transformPosition.x, GetBounds().center.y, 0);
            }
        }

        public void ShowLine()
        {
            TransportUtil.ShowTransportLine(LineID);
        }

        public void HideLine()
        {
            TransportUtil.HideTransportLine(LineID);
        }

        public override void Update()
        {
            base.Update();

            var residents = TransportUtil.GetResidentPassengerCount(LineID);
            var tourists = TransportUtil.GetTouristPassengerCount(LineID);

            var lineName = TransportUtil.GetLineName(LineID);
            if (lineName != LineName)
            {
                var handlers = LineNameChanged;
                if (handlers != null)
                    handlers(LineID);
            }
            LineName = lineName;

            _name.text = LineName.Trim();
            /*bool clipped = false;
            while (_name.width > 110)
            {
                _name.text = _name.text.Remove(_name.text.Length - 1);
                clipped = true;
            }
            if (clipped)
                _name.text = _name.text.Trim() + "...";*/

            _stops.text = Stops.ToString();
            _passengers.text = String.Format("{0}/{1}", residents, tourists);
            _trips.text = String.Format("{0}%", Trips);
            _vehicles.text = Vehicles.ToString();
            _dayOnly.IsChecked = TransportUtil.GetDay(LineID);
            _nightOnly.IsChecked = TransportUtil.GetNight(LineID);

            _color.selectedColor = TransportUtil.GetLineColor(LineID);
            IsChecked = !TransportUtil.IsTransportLineHidden(LineID);
        }
    }
}
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace CustomCalendar
{
    public partial class CustomCalendarView : ContentView
    {
        public CustomCalendarView()
        {
            InitializeComponent();
            CurrentDate = DateTime.Today;
            DrawCalendar();
        }

        public static readonly BindableProperty SelectedDatesProperty =
        BindableProperty.Create(
            nameof(SelectedDates),
            typeof(List<DateTime>),
            typeof(CustomCalendarView),
            defaultValue: new List<DateTime>(),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is CustomCalendarView calendar)
                {
                    calendar.DrawCalendar();
                }
            });

        public static readonly BindableProperty ZileRestanteProperty =
            BindableProperty.Create(
                nameof(ZileRestante),
                typeof(List<DateTime>),
                typeof(CustomCalendarView),
                defaultValue: new List<DateTime>(),
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    if (bindable is CustomCalendarView calendar)
                    {
                        calendar.DrawCalendar();
                    }
                });

     

        public List<DateTime> SelectedDates
        {
            get => (List<DateTime>)GetValue(SelectedDatesProperty);
            set => SetValue(SelectedDatesProperty, value);
        }

        public List<DateTime> ZileRestante
        {
            get => (List<DateTime>)GetValue(ZileRestanteProperty);
            set => SetValue(ZileRestanteProperty, value);
        }

        public static readonly BindableProperty DayTappedCommandProperty =
            BindableProperty.Create(
                nameof(DayTappedCommand),
                typeof(ICommand),
                typeof(CustomCalendarView),
                default(ICommand),
                propertyChanged: OnDayTappedCommandChanged);

        public ICommand DayTappedCommand
        {
            get => (ICommand)GetValue(DayTappedCommandProperty);
            set => SetValue(DayTappedCommandProperty, value);
        }

        private static void OnDayTappedCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomCalendarView calendar)
            {
                calendar.DrawCalendar(); 
            }
        }

        public static readonly BindableProperty ShowPastDatesCircledProperty =
    BindableProperty.Create(
        nameof(ShowPastDatesCircled),
        typeof(bool),
        typeof(CustomCalendarView),
        true, 
        propertyChanged: (_, __, ___) => ((CustomCalendarView)___).DrawCalendar()
    );

        public bool ShowPastDatesCircled
        {
            get => (bool)GetValue(ShowPastDatesCircledProperty);
            set => SetValue(ShowPastDatesCircledProperty, value);
        }

        private DateTime CurrentDate;

        private void OnPreviousMonthClicked(object sender, EventArgs e)
        {
            CurrentDate = CurrentDate.AddMonths(-1);
            DrawCalendar();
        }

        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            CurrentDate = CurrentDate.AddMonths(1);
            DrawCalendar();
        }
        private void DrawCalendar()
        {
            CalendarGrid.Children.Clear();
            CalendarGrid.RowDefinitions.Clear();
            CalendarGrid.ColumnDefinitions.Clear();

            MonthLabel.Text = CurrentDate.ToString("MMMM yyyy", new CultureInfo("ro-RO"));

            // Define 7 columns for 7 days
            for (int i = 0; i < 7; i++)
            {
                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }
          
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int i = 0; i < 6; i++)
            {
                CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

         
            string[] daysOfWeek = new[] { "L", "Ma", "Mi", "J", "V", "S", "D" };
            for (int col = 0; col < 7; col++)
            {
                var dayLabel = new Label
                {
                    Text = daysOfWeek[col],
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Colors.Black
                };
                CalendarGrid.Add(dayLabel, col, 0);
            }

        
            var firstDayOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            int skipDays = ((int)firstDayOfMonth.DayOfWeek - 1 + 7) % 7; 
            int daysInMonth = DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month);

            int dayCounter = 1;
            //start time 

            for (int row = 1; row <= 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    int dayIndex = (row - 1) * 7 + col;

                    if (dayIndex < skipDays || dayCounter > daysInMonth)
                        continue;

                    var date = new DateTime(CurrentDate.Year, CurrentDate.Month, dayCounter);

                    Color background = Colors.Transparent;
                    Color borderColor = Colors.Transparent;
                    if (SelectedDates?.Any(d => d.Date == date.Date) == true)
                    { background = Colors.LightBlue;
                        if (date < DateTime.Now && ShowPastDatesCircled) { borderColor = Colors.DarkBlue; background = Colors.Transparent; }
                    }
                    else if (ZileRestante?.Any(d => d.Date == date.Date) == true)
                        background = Colors.AliceBlue;

                    var label = new Label
                    {
                        Text = date.Day.ToString(),
                        TextColor = Colors.Black,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };

                    var frame = new Frame
                    {
                        CornerRadius = 20,
                        BackgroundColor = background, 
                        BorderColor= borderColor,
                        Content = label,
                        Padding = 5,
                        HasShadow = false,
                        Margin = 2,
                      


                    };

                    var tapGestureRecognizer = new TapGestureRecognizer
                    {
                        Command = DayTappedCommand,
                        CommandParameter = date
                    };

                    frame.GestureRecognizers.Add(tapGestureRecognizer);
                    CalendarGrid.Add(frame, col, row);

                    dayCounter++;
                }
            }

            //end time 
        }


    }
}

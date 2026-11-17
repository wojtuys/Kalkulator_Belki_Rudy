using System;
using System.Globalization;

namespace KalkulatorBelki;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        materialPicker.SelectedIndex = 0;
    }

    private void Stepper_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        liczbaLabel.Text = $"Liczba belek: {(int)e.NewValue}";
    }

    private void Oblicz_Clicked(object sender, EventArgs e)
    {
        // Sprawdzanie poprawności wymiarów
        if (!double.TryParse(dlugoscEntry.Text, NumberStyles.Float,
                CultureInfo.CurrentCulture, out double L) ||
            !double.TryParse(szerokoscEntry.Text, NumberStyles.Float,
                CultureInfo.CurrentCulture, out double b) ||
            !double.TryParse(wysokoscEntry.Text, NumberStyles.Float,
                CultureInfo.CurrentCulture, out double h) ||
            L <= 0 || b <= 0 || h <= 0)
        {
            raportLabel.Text = "Błąd! Wszystkie wymiary muszą być liczbami dodatnimi.";
            return;
        }

        if (materialPicker.SelectedIndex == -1)
        {
            raportLabel.Text = "Wybierz materiał!";
            return;
        }

        // Gęstość materiału
        string material = materialPicker.SelectedItem.ToString();

        double gestosc = material switch
        {
            "Stal" => 7850,
            "Aluminium" => 2700,
            "Drewno" => 600,
            _ => 0
        };

        // Pole przekroju w m2
        double pole = (b * h) / 10000;

        // Objętość jednej belki
        double objetosc = pole * L;

        // Masa jednej belki
        double masa = objetosc * gestosc;

        // Liczba belek
        int liczba = (int)liczbaStepper.Value;

        // Łączne parametry
        double objetoscLaczna = objetosc * liczba;
        double masaLaczna = masa * liczba;

        // Uwzględnienie zapasu 10%
        if (zapasSwitch.IsToggled)
        {
            objetoscLaczna *= 1.10;
            masaLaczna *= 1.10;
        }

        // Opcjonalne obliczenie ceny
        string kosztRaport = "";

        if (!string.IsNullOrWhiteSpace(cenaEntry.Text))
        {
            if (!double.TryParse(cenaEntry.Text, NumberStyles.Float,
                    CultureInfo.CurrentCulture, out double cena) ||
                cena < 0)
            {
                raportLabel.Text = "Błąd! Cena musi być liczbą nieujemną.";
                return;
            }

            double koszt = masaLaczna * cena;

            kosztRaport =
                $"\nCena za kg: {cena:F2} zł" +
                $"\nSzacunkowy koszt zakupu: {koszt:F2} zł";
        }

        // Raport wyników
        raportLabel.Text =
            $"MATERIAŁ: {material}\n\n" +
            $"Długość belki: {L:F2} m\n" +
            $"Szerokość: {b:F2} cm\n" +
            $"Wysokość: {h:F2} cm\n" +
            $"Gęstość: {gestosc:F0} kg/m³\n\n" +

            $"Pole przekroju: {pole:F4} m²\n" +
            $"Objętość jednej belki: {objetosc:F4} m³\n" +
            $"Masa jednej belki: {masa:F2} kg\n\n" +

            $"Liczba belek: {liczba}\n" +
            $"Zapas 10%: {(zapasSwitch.IsToggled ? "Tak" : "Nie")}\n\n" +

            $"Łączna objętość: {objetoscLaczna:F4} m³\n" +
            $"Łączna masa: {masaLaczna:F2} kg\n" +
            kosztRaport;
    }

    private void Wyczysc_Clicked(object sender, EventArgs e)
    {
        dlugoscEntry.Text = "";
        szerokoscEntry.Text = "";
        wysokoscEntry.Text = "";

        materialPicker.SelectedIndex = 0;

        liczbaStepper.Value = 1;
        liczbaLabel.Text = "Liczba belek: 1";

        zapasSwitch.IsToggled = false;

        cenaEntry.Text = "";

        raportLabel.Text = "Wprowadź dane i naciśnij Oblicz.";
    }
}

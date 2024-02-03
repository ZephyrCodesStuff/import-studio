using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Import.Core;

namespace Import.Components {
    public class LearnTab: UserControl {
        public LearnTab() => AvaloniaXamlLoader.Load(this);

        void Docs() => App.URL("https://github.com/mat1jaczyyy/import-studio/wiki");

        void Tutorials() => App.URL("https://www.youtube.com/playlist?list=PLKC4R3X00beY0aB_f_ZIa3shqJX7do4mH");

        void Bug() => App.URL("https://github.com/mat1jaczyyy/import-studio/issues/new?assignees=mat1jaczyyy&labels=bug&template=bug_report.md&title=");

        void Feature() => App.URL("https://github.com/mat1jaczyyy/import-studio/issues/new?assignees=mat1jaczyyy&labels=enhancement&template=feature_request.md&title=");

        void Question() => App.URL("https://github.com/mat1jaczyyy/import-studio/issues/new?assignees=mat1jaczyyy&labels=question&template=question.md&title=");

        void Discord() => App.URL("https://discordapp.com/invite/2ZSHYHA");

        void Website() => App.URL("https://import.mat1jaczyyy.com");

        void Donate() => App.URL(PayPal.URL);
    }
}

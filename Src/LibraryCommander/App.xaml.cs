using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using BusinessLogic;
using BusinessLogic.Fs;
using DataAccess;
using DataSchema;
using LibraryCommander.Dialogs;
using LibraryCommander.Localization;
using SimpleInjector;
using ViewModels;
using ViewModels.Dialogs;

namespace LibraryCommander
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private VisualDialogContainer _dialogs;

        private CommanderVm _commander;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            string library = ConfigurationManager.AppSettings["library"];

            var args = Environment.GetCommandLineArgs();
            Resources.MergedDictionaries.Clear();
            // apply a skin
            if (args.Length > 1 && args[1] == "modern")
                Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/LibraryCommander;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute) });
            else
            {
                Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/RetroUI;component/Themes/Colors.Retro.xaml", UriKind.RelativeOrAbsolute) });
                Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/RetroUI;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute) });
            }
            
            // configurate dialog implementations
            _dialogs = new VisualDialogContainer();
            _dialogs.Set<WpfMessageBox>("messageBox");
            _dialogs.Set<InputBox>("inputBox");
            _dialogs.Set<SelectorWindow>("selection");
            _dialogs.Set<BookCardWindow>("bookCard");
            _dialogs.Set<WpfOpenFileDialog>("ofd");

            Injector.Dialogs = _dialogs;

            // configurate repository implementations
            var c = new Container();
            c.Register(() => new AuthorManager { Repository = new EntityRepository<Author>() });
            c.Register(() => new CategoryManager { Repository = new CategoryRepository() });            
            c.Register(() => new TagManager { Repository = new EntityRepository<Tag>() });
            c.Register(() => new CycleManager { Repository = new EntityRepository<Cycle>() });
            c.Register(() => new FileLanguageManager { Repository = new FileLanguageRepository() });
            c.Register(() => new FileFormatManager { Repository = new FileFormatRepository() }); 
            c.Register(() => new BookManager { Repository = new BookRepository() });

            c.Register(() => new SearchNavigator(library) { Repository = new BookRepository() });             
            c.Register(() => new VirtualFsNavigator(library)
                             {
                                 BookRepository = new BookRepository(),
                                 CategoryRepository = new CategoryRepository()
                             });
            Injector.Container = c;
            
            var settings = new EntityRepository<AppSettings>().Query().FirstOrDefault();

            // configurate localization
            if (settings != null && String.IsNullOrWhiteSpace(settings.Culture) == false)
                LocalizationProvider.CurrentCulture = new CultureInfo(settings.Culture);
            else
                LocalizationProvider.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            BusinessLogic.Localization.Instance = new LocalizationModel();
            Injector.Localization = new LocalizationVm();            

            StorageManager.StoragePath = library;
            UsageValidator.Repository = new BookRepository();
            
            // configurate main page vm
            _commander = new CommanderVm();

            _commander.CultureChanged += (o, eventArgs) =>
            {
                var vm = (CommanderVm) o;
                LocalizationProvider.CurrentCulture = new CultureInfo(vm.CurrentCulture);
            };

            var p = Directory.GetLogicalDrives()
                .Where(d => Directory.Exists(d))
                .Select((d, i) => new PartitionVm { Name = d, Index = i + 1 });
            _commander.Partitions = new ObservableCollection<PartitionVm>(p);   
            _commander.ApplySettings(settings);
                        
            var w = new MainWindow();
            w.ShowDialog(_commander);
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            // save app setting on Exit
            var rep = new EntityRepository<AppSettings>();
            var settings = rep.Query().FirstOrDefault() ?? new AppSettings();

            settings.Culture = LocalizationProvider.CurrentCulture.Name;

            var c = _commander.GetCurrentSettings();
            settings.FsLocation = c.FsLocation;
            settings.LibraryLocation = c.LibraryLocation;

            settings.Category = c.Category;
            settings.Language = c.Language;
            settings.IdAuthor = c.IdAuthor;
            settings.IdCycle = c.IdCycle;

            rep.SaveOrUpdate(settings);
        }
    }
}

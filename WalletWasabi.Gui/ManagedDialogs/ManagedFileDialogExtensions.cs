using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Platform;

namespace WalletWasabi.Gui.ManagedDialogs
{
	[Obsolete]
	public static class ManagedFileDialogExtensions
	{
		private class ManagedSystemDialogImpl : ISystemDialogImpl
		{
			private async Task<string[]> ShowAsync(SystemDialog d, IWindowImpl parent)
			{
				var model = new ManagedFileChooserViewModel((FileSystemDialog)d);

				var dialog = new ManagedFileDialog
				{
					DataContext = model
				};

				string[] result = null;
				model.CompleteRequested += items =>
				{
					result = items;
					dialog.Close();
				};
				model.CancelRequested += dialog.Close;

				await dialog.ShowDialog<object>(parent);
				return result;
			}

			public async Task<string[]> ShowFileDialogAsync(FileDialog dialog, IWindowImpl parent)
			{
				return await ShowAsync(dialog, parent);
			}

			public async Task<string> ShowFolderDialogAsync(OpenFolderDialog dialog, IWindowImpl parent)
			{
				return (await ShowAsync(dialog, parent))?.FirstOrDefault();
			}
		}

		public static TAppBuilder UseManagedSystemDialogs<TAppBuilder>(this TAppBuilder builder)
			where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
		{
			builder.AfterSetup(_ =>
				AvaloniaLocator.CurrentMutable.Bind<ISystemDialogImpl>().ToSingleton<ManagedSystemDialogImpl>());
			return builder;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;

namespace Xamarin.Essentials
{
    public static partial class Share
    {
        static Task PlatformRequestAsync(ShareTextRequest request)
        {
            var items = new List<string>();
            if (!string.IsNullOrWhiteSpace(request.Text))
            {
                items.Add(request.Text);
            }

            if (!string.IsNullOrWhiteSpace(request.Uri))
            {
                items.Add(request.Uri);
            }

            var intent = new Intent(Intent.ActionSend);
            intent.SetType(FileSystem.MimeTypes.TextPlain);
            intent.PutExtra(Intent.ExtraText, string.Join(System.Environment.NewLine, items));

            if (!string.IsNullOrWhiteSpace(request.Subject))
            {
                intent.PutExtra(Intent.ExtraSubject, request.Subject);
            }

            var chooserIntent = Intent.CreateChooser(intent, request.Title ?? string.Empty);
            var flags = ActivityFlags.ClearTop | ActivityFlags.NewTask;
            chooserIntent.SetFlags(flags);
            Platform.AppContext.StartActivity(chooserIntent);

            return Task.CompletedTask;
        }

        static Task PlatformRequestAsync(ShareMultipleFilesRequest request)
        {
            var contentUris = new List<IParcelable>();

            var intentType = request.Files.Count > 1 ? Intent.ActionSendMultiple : Intent.ActionSend;
            var intent = new Intent(intentType);

            foreach (var file in request.Files)
                contentUris.Add(Platform.GetShareableFileUri(file));

            var type = request.Files.Count > 1
                ? FileSystem.MimeTypes.All
                : request.Files.FirstOrDefault().ContentType;
            intent.SetType(type);

            intent.SetFlags(ActivityFlags.GrantReadUriPermission);
            intent.PutParcelableArrayListExtra(Intent.ExtraStream, contentUris);

            if (!string.IsNullOrEmpty(request.Title))
                intent.PutExtra(Intent.ExtraTitle, request.Title);

            var chooserIntent = Intent.CreateChooser(intent, request.Title ?? string.Empty);
            var flags = ActivityFlags.ClearTop | ActivityFlags.NewTask;
            chooserIntent.SetFlags(flags);
            Platform.AppContext.StartActivity(chooserIntent);

            return Task.CompletedTask;
        }
    }
}

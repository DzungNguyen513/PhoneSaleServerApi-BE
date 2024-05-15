using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace PhoneSaleAPI.Firebase
{
	public class FirebaseManager
	{
		private static FirebaseApp app;

		public static void InitializeFirebaseApp()
		{
			if (FirebaseApp.DefaultInstance == null)
			{
				string pathToJson = @"D:\API\BTL-API\PhoneSaleAPI\PhoneSaleAPI\phonesaleapp-firebase.json";
				app = FirebaseApp.Create(new AppOptions
				{
					Credential = GoogleCredential.FromFile(pathToJson)
				});
			}
		}

		public static async Task<string> SendNotificationToTopic(string topic, string title, string body)
		{
			var message = new Message()
			{
				Topic = topic,
				Notification = new Notification
				{
					Title = title,
					Body = body
				}
			};

			try
			{
				return await FirebaseMessaging.DefaultInstance.SendAsync(message);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("Test lỗi: " + ex.Message);
			}
		}

		public static async Task<string> SendNotificationToToken(string token, string title, string body)
		{
			var message = new Message()
			{
				Token = token,
				Notification = new Notification
				{
					Title = title,
					Body = body
				}
			};

			try
			{
				return await FirebaseMessaging.DefaultInstance.SendAsync(message);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("Lỗi gửi message: " + ex.Message);
			}
		}
	}

}
﻿using System;
using Akka.Actor;
using Akka.Event;
using HabitableZone.Core.ClientServerMessages;

namespace HabitableZone.Server
{
	/// <inheritdoc />
	/// <summary>
	///    Handles communication with connected client.
	/// </summary>
	internal class ClientSessionActor : ReceiveActor
	{
		private readonly IActorRef _remoteActorRef;
		private readonly Guid _playerGuid;
		private readonly String _nick;
		private readonly ILoggingAdapter _logger = Context.GetLogger();

		public ClientSessionActor(Guid playerGuid, String nick, IActorRef remoteActorRef)
		{
			_playerGuid = playerGuid;
			_nick = nick;
			_remoteActorRef = remoteActorRef;

			Context.Watch(_remoteActorRef);

			Receive<ClientSessionActorMessages.DisconnectRequest>(message =>
			{
				Sender.Tell(new ServerConnectionActorMessages.DisconnectResponce());
				OnDisconnect();
			});

			Receive<Terminated>(message => OnDisconnect());
		}

		public static Props Props(Guid playerGuid, String nick, IActorRef remoteActorRef)
		{
			return Akka.Actor.Props.Create(() => new ClientSessionActor(playerGuid, nick, remoteActorRef));
		}

		private void OnDisconnect()
		{
			Context.Stop(Self);
			_logger.Info($"Stopped, client {_nick} disconnected.");
		}
	}
}
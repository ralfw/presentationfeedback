
namespace EventStore.Internals
{
	using System;
	using System.Threading;

	internal static class ReaderWriterLockSlimExtensions
	{
		private sealed class ReadLockToken : IDisposable
		{
			private ReaderWriterLockSlim rwLock;

			public ReadLockToken(ReaderWriterLockSlim rwLock)
			{
				this.rwLock = rwLock;
				rwLock.EnterReadLock();
			}

			public void Dispose()
			{
				if (rwLock == null) return;
				rwLock.ExitReadLock();
				rwLock = null;
			}
		}

		private sealed class WriteLockToken : IDisposable
		{
			private ReaderWriterLockSlim rwLock;

			public WriteLockToken(ReaderWriterLockSlim rwLock)
			{
				this.rwLock = rwLock;
				rwLock.EnterWriteLock();
			}

			public void Dispose()
			{
				if (rwLock == null) return;
				rwLock.ExitWriteLock();
				rwLock = null;
			}
		}

		public static IDisposable Read(this ReaderWriterLockSlim rwLock)
		{
			return new ReadLockToken(rwLock);
		}

		public static IDisposable Write(this ReaderWriterLockSlim rwLock)
		{
			return new WriteLockToken(rwLock);
		}
	}
}

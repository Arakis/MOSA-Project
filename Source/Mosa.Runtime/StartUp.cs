// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Runtime
{

	/// <summary>
	/// Basic Runtime Initialization Routines
	/// </summary>
	public static class StartUp
	{

		/// <summary>
		/// This is the main entry point.
		/// </summary>
		public static void Initialize()
		{
			KernelInitialization();
			SetInitialMemory();
			InitializeAssembly();
			InitializeRuntimeMetadata();
			StartApplication();
		}

		/// <summary>
		/// Setup SSE and other stuff
		/// This Method is plugged.
		/// </summary>
		public static void KernelInitialization()
		{
		}

		/// <summary>
		/// Setup early Boot Memory
		/// This Method is plugged.
		/// </summary>
		public static void SetInitialMemory()
		{
		}

		/// <summary>
		/// Initialize static fields
		/// This Method is plugged.
		/// </summary>
		public static void InitializeAssembly()
		{
		}

		/// <summary>
		/// Initialize the type system.
		/// This Method is plugged.
		/// </summary>
		public static void InitializeRuntimeMetadata()
		{
		}

		/// <summary>
		/// This Method is plugged and will call the .NET Main method
		/// </summary>
		public static void StartApplication()
		{
		}
	}
}

# ParallelJobRunner

[![.NET](https://img.shields.io/badge/.NET-6.0%2B-blueviolet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**ParallelJobRunner** is a .NET library for managing asynchronous background jobs. It provides a robust framework for executing tasks in parallel, with support for cancellation, detailed logging, user-specified return values, and graceful shutdown handling. The library is designed for applications requiring concurrent task execution, such as Windows Forms apps, console applications, or services.

## Features

- **Asynchronous Job Execution**: Run tasks in the background with support for async/await.
- **Cancellation Support**: Cancel individual jobs or all jobs with a single method call.
- **Detailed Logging**: Log job events to memory and a file, with real-time log retrieval.
- **User-Specified Return Values**: Allow users to define custom return values for job results.
- **Result Handling**: Store and retrieve job results for completed tasks.
- **Shutdown Prevention**: Prevent application shutdown if jobs are running, with options to cancel or wait.
- **Thread-Safe Operations**: Ensure safe access to job collections and logs in multi-threaded environments.
- **XML Documentation**: Comprehensive code documentation for IntelliSense and API reference.
- **Observable Job Collection**: Bind job status and results to UI components (e.g., DataGridView).

## Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/yourusername/ParallelJobRunner.git
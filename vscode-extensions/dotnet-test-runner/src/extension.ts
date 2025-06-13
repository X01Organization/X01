import * as vscode from "vscode";

export function activate(context: vscode.ExtensionContext) {
  const taskProvider = vscode.tasks.registerTaskProvider("X01", {
    provideTasks: () => {
      return []; // optional predefined tasks
    },
    resolveTask,
  });

  context.subscriptions.push(taskProvider);
}

function resolveTask(task: vscode.Task): vscode.Task | undefined {
  const definition = task.definition as any;

  const command = definition.command;
  const args = definition.args || [];

  if (!command) {
    return undefined;
  }

  // Join command and args for full command string
  const fullCommand = [command, ...args].join(" ");
  console.log(`Resolved task command: ${fullCommand}`); // Logs to Developer Tools console
  vscode.window.showInformationMessage(`Running: ${fullCommand}`); // Optional popup

  const shellExec = new vscode.ShellExecution(command, args);
  return new vscode.Task(
    definition,
    task.scope ?? vscode.TaskScope.Workspace,
    task.name,
    "X01",
    shellExec,
    task.problemMatchers
  );
}

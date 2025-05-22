/**
 *****************************************
 * Created by edonet@163.com
 * Created on 2020-05-30 20:05:28
 *****************************************
 */
'use strict';


/**
 *****************************************
 * 加载依赖
 *****************************************
 */
import * as vscode from 'vscode';
import Command, { TerminalOptions } from './Command';
import { get } from 'http';


/**
 *****************************************
 * 命令配置
 *****************************************
 */
export interface CommandOptions {
    cmd?: string;
    command?: string;
    terminal?: string | TerminalOptions;
    appendExplorerSelectedFiles?: boolean;
}


/**
 *****************************************
 * 激活扩展
 *****************************************
 */
function getConfiguredCommands(): { command: string; title: string }[] {
  const config = vscode.workspace.getConfiguration('x01');
  const commands = config.get<{ command: string; title: string }[]>('command-runner.commands') ?? [];
  return commands;
}
export function activate(context: vscode.ExtensionContext): void {

    // 注册【运行】命令
    context.subscriptions.push(
        vscode.commands.registerCommand('x01.command-runner.run',
          async (...args: any[]) => {
  let opts: any;
  let contextSelection: vscode.Uri | undefined;
  let allSelections: vscode.Uri[] = [];

  // If only one argument and it's an object, it's from keybinding
  if (args.length === 1 && typeof args[0] === 'object' && !(args[0] instanceof vscode.Uri)) {
    opts = args[0];
  } else {
    contextSelection = args[0] as vscode.Uri;
    allSelections = args[1] as vscode.Uri[] || [];
    opts = args[2];
  }
  // Debug:
  console.log('contextSelection:', contextSelection?.fsPath);
  console.log('allSelections:', allSelections?.map(u => u.fsPath));
  console.log('opts:', opts);

  let cc = getConfiguredCommands();
  console.log('commands11:', cc);

  let c = new Command(context);
  c.pick();
})
    );


    // 注册【在终端运行】命令
    context.subscriptions.push(
        vscode.commands.registerCommand('x01.command-runner.runInTerminal', ({ terminal }: CommandOptions = {}) => {
            const command = new Command(context);

            // 兼容终端名参数
            if (typeof terminal === 'string') {
                terminal = { name: terminal };
            }

            // 执行命令
            command.executeSelectText(terminal);
        })
    );
}

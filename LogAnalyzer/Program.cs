using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

class LogAnalyzer
{
    // Conjunto para armazenar erros já exibidos e evitar repetições
    private HashSet<string> displayedErrors = new HashSet<string>();
    private string logFilePath = "log.txt";  // Caminho para o arquivo de log local

    public void ProcessLogs(string logs)
    {
        // Defina os padrões que você quer capturar, por exemplo, palavras-chave como "ERROR", "Exception" ou "Falha"
        string errorPattern = @"(ERROR|Exception|FAILURE|CRITICAL)"; // Regex para capturar erros
        string causePattern = @"(?:caused by|root cause|reason:)\s*(.*)"; // Captura a causa do erro, ajustável conforme o formato dos logs

        // Processar as linhas dos logs
        var logLines = logs.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        // StringBuilder para armazenar a saída formatada
        var result = new StringBuilder();
        result.AppendLine("Analisando os logs...");
        result.AppendLine();

        // Para cada linha do log, verifica se contém erros
        foreach (var line in logLines)
        {
            // Verifica se a linha contém um erro
            if (Regex.IsMatch(line, errorPattern, RegexOptions.IgnoreCase))
            {
                // Se já mostramos esse erro, ignora e não repete
                if (displayedErrors.Contains(line))
                {
                    continue;
                }

                // Marca o erro como já exibido
                displayedErrors.Add(line);

                // Exibe o erro em vermelho no console
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Erro Detectado]: " + line);
                result.AppendLine("[Erro Detectado]: " + line); // Adiciona ao resultado para o arquivo

                // Adiciona um espaçamento
                Console.WriteLine();
                result.AppendLine();

                // Procura pela causa do erro
                var match = Regex.Match(line, causePattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    // Exibe a causa do erro em amarelo no console
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  Causa identificada: " + match.Groups[1].Value);
                    result.AppendLine("  Causa identificada: " + match.Groups[1].Value); // Adiciona ao resultado para o arquivo
                }
                else
                {
                    Console.WriteLine("  Causa não identificada diretamente.");
                    result.AppendLine("  Causa não identificada diretamente.");
                }

                // Reseta a cor para o padrão no console
                Console.ResetColor();

                // Adiciona um espaçamento entre os erros para melhor organização
                Console.WriteLine();
                result.AppendLine();
            }
        }

        // Salva os resultados no arquivo de log (substituindo o conteúdo)
        File.WriteAllText(logFilePath, result.ToString());
    }
}

class Program
{
    static void Main(string[] args)
    {
        var logAnalyzer = new LogAnalyzer();

        while (true)
        {
            // Pede para o usuário colar os logs
            Console.Clear();
            Console.WriteLine("Cole os logs abaixo. Pressione Enter para finalizar a entrada.");
            string logs = ReadLogsFromConsole();

            // Processa os logs e salva no arquivo log.txt
            logAnalyzer.ProcessLogs(logs);

            // Após o processamento, limpa a tela e aguarda o próximo
            Console.WriteLine("Análise salva em log.txt.");
            Console.WriteLine("Pressione Enter para colar mais logs ou Ctrl+C para sair.");
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                continue; // Limpa a tela e permite colar novos logs
            }
            else
            {
                break; // Sai do programa caso o usuário pressione Ctrl+C ou outra tecla
            }
        }
    }

    // Função para ler os logs copiados e colados no console
    static string ReadLogsFromConsole()
    {
        string logs = "";
        string line;

        while ((line = Console.ReadLine()) != null && line.Length > 0)
        {
            logs += line + "\n";
        }

        return logs;
    }
}

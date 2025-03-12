# 🏛️ **migracao-patrim**

## 📌 Sobre o Projeto

O **migracao-patrim** é uma aplicação **.NET Framework Console** que automatiza a extração, processamento e migração de dados patrimoniais.

🔹 Extrai informações do **Sybase**  
🔹 Processa e trata os dados  
🔹 Armazena no **PostgreSQL**  
🔹 Refina os dados para envio ao **Cloud Betha**

A solução foi desenvolvida para garantir um fluxo automatizado e seguro de migração de informações patrimoniais, garantindo integridade e confiabilidade dos dados.

## 🚀 Tecnologias Utilizadas

- **C# .NET Framework**
- **Dapper** (ORM leve para acesso ao PostgreSQL)
- **Newtonsoft.Json** (Manipulação de JSON)
- **Npgsql** (Driver para PostgreSQL)
- **API REST** (Integração com o Cloud Betha)

## 📂 Estrutura do Projeto

```bash
📦 migracao-patrim
├── 📁 Connections       # Gerenciamento de conexões com banco de dados
├── 📁 Controller        # Camada de controle da aplicação
├── 📁 DownloadGLB       # Processos de download de dados GLB
├── 📁 DownloadSybase    # Processos de download do Sybase
├── 📁 Models            # Modelos de dados
├── 📁 Properties        # Configurações do projeto
├── 📁 Request           # Camada de requisições e comunicação
├── 📁 SQL              # Scripts SQL para manipulação dos dados
├── 📁 UseCase          # Casos de uso e regras de negócio
├── 📁 Utils            # Métodos auxiliares e utilitários
├── 📄 Program.cs        # Ponto de entrada da aplicação
├── 📄 App.config        # Configuração da aplicação
├── 📄 packages.config   # Dependências do projeto
├── 📄 migracao-patrim.sln # Solução do Visual Studio
├── 📄 README.md         # Documentação do projeto
```

## 🔄 Fluxo de Funcionamento

1️⃣ Extrai os dados do **Sybase**  
2️⃣ Processa e **valida as informações**  
3️⃣ Persiste os dados no **PostgreSQL**  
4️⃣ Refina os dados para envio ao **Cloud Betha**

## 🛠️ Como Executar o Projeto

### 1️⃣ Clonar o Repositório

```bash
git clone https://github.com/nathaliacosim/migracao-patrim.git
cd migracao-patrim
```

### 2️⃣ Configurar as Dependências

- Instalar o **.NET Framework** compatível
- Configurar as **credenciais da API** no `App.config`
- Garantir que o **PostgreSQL** esteja rodando

### 3️⃣ Rodar a Aplicação

```bash
dotnet run
```

## 📌 Melhorias Futuras

- 🔹 Refinar os envios
- 🔹 Criar mais **classes modelo** para atender diversos cenários
- 🔹 Melhorar o tratamento de erros e logs
- 🔹 Implementar testes unitários

## 📜 Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

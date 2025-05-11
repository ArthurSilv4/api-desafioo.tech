# Desafioo.tech - API

[![License](https://img.shields.io/github/license/ArthurSilv4/api-desafioo.tech)](https://opensource.org/licenses/MIT)
[![Stars](https://img.shields.io/github/stars/arthursilv4/api-desafioo.tech)](https://github.com/arthursilv4/api-desafioo.tech)
[![Documentation](https://img.shields.io/badge/Documentation-Online-brightgreen)](https://arthursilv4.github.io/api-desafioo.tech/)

**Desafioo.tech** é uma plataforma open-source para criação e resolução de desafios de programação, conectando profissionais, professores e estudantes.

> **Este repositório contém apenas o backend da plataforma.**  
> Para acessar o frontend, visite: [https://github.com/ArthurSilv4/front-desafioo.tech](https://github.com/ArthurSilv4/front-desafioo.tech)

Acesse a plataforma: [https://desafioo.tech](https://desafioo.tech)

---

## Sumário

1. [Funcionalidades](#1---funcionalidades)
2. [Tecnologias](#2---tecnologias)
3. [Arquitetura](#3---arquitetura)
4. [Instalação](#4---instalação)
5. [Apoiadores](#5---apoiadores)
6. [Contribuições](#6---contribuições)
7. [Contato](#7---contato)

---

## 1 - Funcionalidades <a name="1---funcionalidades"></a>

- Cadastro e autenticação de usuários (JWT)
- Criação, edição e remoção de desafios por professores/profissionais
- Exploração e inscrição em desafios por usuários
- Envio de notificações por e-mail (SMTP)
- Feedbacks e comentários em desafios
- Sistema de cache para performance (Redis)
- Administração de usuários e desafios
- API RESTful documentada

---

## 2 - Tecnologias <a name="2---tecnologias"></a>

- ![C#](https://img.shields.io/badge/C%23-512BD4?style=for-the-badge&logo=csharp&logoColor=white)
- ![ASP.NET](https://img.shields.io/badge/ASP.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
- ![Entity Framework](https://img.shields.io/badge/Entity%20Framework-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
- ![Postgres](https://img.shields.io/badge/Postgres-336791?style=for-the-badge&logo=postgresql&logoColor=white)
- ![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)
- ![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)

---

## 3 - Arquitetura <a name="3---arquitetura"></a>

```text
                ┌───────────────────────────┐
                │         Front-end         │
                │          (React)          │
                └────────────┬──────────────┘
                             │
                     🔐 Auth Token (JWT)
                             │
                ┌────────────▼──────────────┐
                │         API .NET          │
                │   (.NET Core / ASP.NET)   │
                └────────────┬──────────────┘
                             │
        ┌────────────────────┼────────────────────┐
        │                    │                    │
┌───────▼───────┐    ┌───────▼────────┐   ┌───────▼───────┐
│   Postgres    │    │     Redis      │   │     SMTP      │
│  (Dados App)  │    │    (Cache)     │   │ (Envio Email) │
└───────────────┘    └────────────────┘   └───────────────┘
```

---

## 4 - Instalação <a name="4---instalação"></a>

Siga os passos abaixo para rodar o projeto localmente:

1. **Clone o repositório:**
    ```bash
    git clone https://github.com/arthursilv4/api-desafioo.tech.git
    ```

2. **Acesse o diretório do projeto:**
    ```bash
    cd api-desafioo.tech
    ```
    - Abra o Visual Studio.
    - Clique em "Open a project or solution".
    - Selecione o arquivo `.sln` do projeto.

3. **Execute o Docker Compose para iniciar os contêineres:**
    ```bash
    docker-compose up -d
    ```

4. **Aplique as migrações do banco de dados:**
    ```bash
    dotnet ef database update
    ```

5. **Execute o projeto:**
    - Selecione o projeto de inicialização no Solution Explorer.
    - Pressione F5 ou clique em "Start".

Pronto! Agora você pode começar a desenvolver e explorar o projeto.

---

## 5 - Apoiadores <a name="5---apoiadores"></a>

Se você acredita no potencial deste projeto e deseja contribuir para seu crescimento, pode apoiar financeiramente através da nossa [**página de apoio**](https://apoia.se/desafiootech).  
Sua contribuição ajudará na manutenção, desenvolvimento de novas funcionalidades e melhorias na plataforma.

**Qualquer valor é bem-vindo e faz a diferença para tornar essa iniciativa ainda mais acessível.**

---

## 6 - Contribuições <a name="6---contribuições"></a>

Contribuições são bem-vindas!  
Abra uma issue, envie um pull request ou compartilhe feedbacks.

---

## 7 - Contato <a name="7---contato"></a>

Se você tiver dúvidas, sugestões ou feedback, por favor, abra uma **issue** neste repositório.  
Fique à vontade para compartilhar suas ideias ou relatar problemas diretamente no [GitHub Issues](https://github.com/arthursilv4/api-desafioo.tech/issues).

---

**Obrigado por acessar o Desafioo.tech!**  
Estamos empolgados em tê-lo aqui e esperamos que você aproveite ao máximo a plataforma.

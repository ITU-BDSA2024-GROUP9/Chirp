---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2024 Group 9
author:
- "Alexander Rossau <ross@itu.dk>"
- "Ayushmaan Bordoloi" <aybo@itu.dk>
- "Bjørn Møgelhøj <bjom@itu.dk>"
numbersections: true
---

# Design and Architecture of _Chirp!_

## Domain model

Here comes a description of our domain model.
OK GOOD
![Illustration of the _Chirp!_ data model as UML class diagram.](images/domain2.svg)

## Architecture — In the small (OK GOOD MAN)
OK GOOD

## Architecture of deployed application (OK GOOD MAN)
OK GOOD

## User activities (JOHN JOHN)

## Sequence of functionality/calls trough _Chirp!_ (BJØRN)
In this section we will detail how the flow of messages and data work in our chirp application.


# Process

## Build, test, release, and deployment (PHILLIP)

## Team work (ALEX)
![Project board](images/board.png)
The above image shows our project board on GitHub. We have used the project board to keep track of our progress and to assign tasks to each other.
The board is divided into four columns: Todo, In Progress, Under review, and Done. Each task is represented by a card that can be moved between the columns.
At the time of writing, we only have one task on the board which is yet to be completed (toggle light/dark mode). We do, however feel it would be nice to have features such as liking posts, so we might add that later.

Feature development workflow:

- Issue created with requirements
- Branch created from main
- Development and testing
- PR created with reviews
- CI checks run
- PR merged to main
- Deployment triggered

## How to make _Chirp!_ work locally (ALEX)

1. Clone the repository:

```bash
git clone https://github.com/ITU-BDSA2024-GROUP9/Chirp.git
cd Chirp
```

2. Set up development environment:

```bash
dotnet restore
```

3. Configure environment:

Create user secrets for GitHub OAuth
```bash
dotnet user-secrets init
dotnet user-secrets set "GITHUBCLIENTID" "YOUR_CLIENT_ID"
dotnet user-secrets set "GITHUBCLIENTSECRET" "YOUR_CLIENT_SECRET"
```

4. Run the application:


```bash
dotnet run --project src/Chirp.Razor
```

5. Access the application:

- Open browser to https://localhost:5273
- Default test accounts available

## How to run test suite locally (PHILLIP AND JONATHAN)

# Ethics (BJØRN)

## License (BJØRN)

## LLMs, ChatGPT, CoPilot, and others (PHILLIP)

During the development of our project, we utilized the following Large Language Models (LLMs):
- Claude
- Microsoft Co-pilot
- OpenAI ChatGPT

### Application of LLMs in Development

The LLMs listed above primarily served as tools for idea and concept generation.
Each week, we were given a list of requirements to complete before the next lecture. When the requirements were precise
and clearly defined, the LLMs played a valuable role in helping us troubleshoot issues or overcome roadblocks.
On the other hand, when requirements were broader or open-ended, the LLMs allowed us to quickly acquire knowledge in the
relevant areas, enabling us to proceed with implementation more confidently.

By Acting as a research and troubleshoot assistant, the LLMs significantly reduced the time we would otherwise have spent
doing traditional searches online. This was particularly useful for resolving highly specific challenges, where the efficiency
and personalized responses from the LLMs saved us considerable effort.

### Reflection on the Helpfulness of LLM responses

The responses provided by the LLMs were highly beneficial for research and troubleshooting, offering us personalized guidance
that gave a better understanding of the subject. THis approach proved more effective than manually going through generalized
search engine results, as the LLMs offered targeted answers specific to our queries. 

However, we found that the LLMs were less effective for directly assisting with coding. While they provided general guidance
and suggestions, they often struggled to fully grasp the context of our project, making it difficult to rely on them for
specific code solutions. Despite this limitation, their overall assistance made it easier for us to progress and quickly
gain necessary information.

### Impact on the development speed

LLMs are great at giving us personalized help, if we ever got stuck or had any questions. The internet typically has
people with similar issues, but it is never quite exactly the same. The LLMs helped a tremendous amount speeding up the
coding, since we did not have to spend a lot of time Google, but instead had a direct go to, which would generate a
personalized reply.


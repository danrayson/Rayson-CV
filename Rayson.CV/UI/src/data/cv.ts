export interface WorkExperience {
  id: string;
  company: string;
  period: string;
  yearStart: number;
  title: string;
  description: string;
  bulletPoints: string[];
}

export interface Project {
  id: string;
  name: string;
  description: string;
  period: string;
  year: number;
  technologies: string[];
  type: 'application' | 'library' | 'tool';
}

export interface SkillCategory {
  id: string;
  name: string;
  skills: string[];
}

export interface PersonalDetails {
  address: string;
  phone: string;
  email: string;
  website: string;
}

export const personalDetails: PersonalDetails = {
  address: 'Stafford, ST17 9SB',
  phone: '07703 574867',
  email: 'daniel@rayson.dev',
  website: 'www.rayson.dev',
};

export const workExperience: WorkExperience[] = [
  {
    id: 'freelance',
    company: 'Freelance',
    period: '2024 - Current',
    yearStart: 2024,
    title: 'Senior Full-Stack Developer',
    description: 'Full-stack development with focus on quality delivery and project ownership',
    bulletPoints: [
      'Followed ISO 29110 processes to ensure quality delivery',
      'Cloud and On-site hosting solutions with full CI/CD pipelines',
      'Full project lifecycle ownership from requirements through deployment',
      'Completed full-stack applications: arbitrage platform and custom e-commerce shop',
      'Contract and legal compliance considerations in project delivery',
      'Self-hosted portfolio website (www.rayson.dev) and additional personal projects',
    ],
  },
  {
    id: 'the-site-doctor',
    company: 'The Site Doctor (Small and Highly Skilled Software House)',
    period: '2021 - 2024',
    yearStart: 2021,
    title: 'Software Developer',
    description: 'Full project ownership in a small, highly skilled software house',
    bulletPoints: [
      'Full project ownership, requirement gathering, work specification, deployment and support',
      'Supporting and advising the development team',
      'Developed B2B APIs deployed to the Cloud using CI',
      'Responsible for communicating with clients and work team',
    ],
  },
  {
    id: 'inspired-gaming',
    company: 'Inspired Gaming UK Ltd (Video Streaming and Back-end Services)',
    period: '2017 - 2021',
    yearStart: 2017,
    title: 'Senior Software Developer',
    description: 'Senior developer role in video streaming and back-end services',
    bulletPoints: [
      'Development ownership of solo-projects',
      'Supporting and advising the development team',
      'Developed micro-services, video streaming, and various UI technologies',
      'Unit, Behaviour, and Integration testing using various suites',
      'Completed multiple projects through to production',
    ],
  },
  {
    id: 'arvs',
    company: 'ARVS ltd (Car Accident Claims Management)',
    period: '2014 - 2016',
    yearStart: 2014,
    title: 'Web Developer',
    description: 'Web developer supporting a bespoke motor accident management website',
    bulletPoints: [
      'Supporting and maintaining a bespoke motor accident management website',
      'Creating add-ons and new services in an agile environment',
      'Advisor to junior members of staff and introducer of new technologies to the team',
      'Consulted with end users to achieve the best results',
    ],
  },
  {
    id: 'dblogic',
    company: 'DBLogic (Software House)',
    period: '2012 - 2014',
    yearStart: 2012,
    title: 'Software Development Engineer',
    description: 'Software development engineer in a software house environment',
    bulletPoints: [
      'Full development life-cycle ownership',
      'Customer support and software bug fixing',
      'Experience with the SCRUM method',
      'Multiple completed business applications',
    ],
  },
  {
    id: 'freelance',
    company: 'Freelance (Personal and Brochure Websites)',
    period: '2007 - 2012',
    yearStart: 2007,
    title: 'Website Development and Support',
    description: 'Freelance website development and support',
    bulletPoints: [
      'Design and implementation of custom websites',
      'General site maintenance and updates',
      'Site specification and general IT consultation',
    ],
  },
];

export const projects: Project[] = [
  {
    id: 'rayson-arbitrage',
    name: 'Rayson.Arbitrage',
    description: 'Arbitrage trading system',
    period: '2025',
    year: 2025,
    technologies: ['.NET', 'Trading'],
    type: 'application',
  },
  {
    id: 'rayson-priceprediction',
    name: 'Rayson.PricePrediction',
    description: 'Price prediction ML system',
    period: '2025',
    year: 2025,
    technologies: ['.NET', 'ML', 'Python'],
    type: 'application',
  },
  {
    id: 'rayson-ollama',
    name: 'Rayson.Ollama',
    description: 'Ollama LLM integration',
    period: '2025',
    year: 2025,
    technologies: ['.NET', 'Ollama', 'LLM'],
    type: 'library',
  },
  {
    id: 'agenticflow',
    name: 'AgenticFlow',
    description: 'AI agent framework',
    period: '2024',
    year: 2024,
    technologies: ['AI Agents', 'LLM'],
    type: 'application',
  },
  {
    id: 'cursedevolution',
    name: 'CursedEvolution',
    description: 'Evolution algorithm experiments',
    period: '2024',
    year: 2024,
    technologies: ['.NET', 'Evolutionary Algorithms'],
    type: 'application',
  },
  {
    id: 'tradepulse',
    name: 'TradePulse',
    description: 'Trading API project',
    period: '2023',
    year: 2023,
    technologies: ['.NET', 'API'],
    type: 'application',
  },
  {
    id: 'tradepulse3',
    name: 'TradePulse3',
    description: 'Trading system v3',
    period: '2024',
    year: 2024,
    technologies: ['.NET', 'React'],
    type: 'application',
  },
  {
    id: 'rayson-cv',
    name: 'Rayson.CV',
    description: 'Personal CV website',
    period: '2025',
    year: 2025,
    technologies: ['React', 'TypeScript', 'Vite'],
    type: 'application',
  },
  {
    id: 'rayson-dev',
    name: 'Rayson.dev',
    description: 'Personal portfolio website',
    period: '2022',
    year: 2022,
    technologies: ['React', 'TypeScript'],
    type: 'application',
  },
  {
    id: 'rayson-portfolio',
    name: 'Rayson.Portfolio',
    description: 'Portfolio website',
    period: '2022',
    year: 2022,
    technologies: ['.NET', 'React', 'MAUI'],
    type: 'application',
  },
  {
    id: 'rayson-studio',
    name: 'Rayson.Studio',
    description: 'Web studio application',
    period: '2021',
    year: 2021,
    technologies: ['.NET', 'React'],
    type: 'application',
  },
  {
    id: 'revolve',
    name: 'rEvolve',
    description: 'Modern evolution application',
    period: '2021',
    year: 2021,
    technologies: ['.NET', 'DDD'],
    type: 'application',
  },
  {
    id: 'rayson-raymbse',
    name: 'Rayson.RayMBSE',
    description: 'Model-Based Systems Engineering',
    period: '2020',
    year: 2020,
    technologies: ['.NET', 'MBSE'],
    type: 'application',
  },
  {
    id: 'rayson-onion',
    name: 'Rayson.Onion',
    description: 'Onion architecture framework',
    period: '2020',
    year: 2020,
    technologies: ['.NET', 'Architecture'],
    type: 'library',
  },
  {
    id: 'rayson-webcrawling',
    name: 'Rayson.WebCrawling',
    description: 'Web crawling utilities',
    period: '2019',
    year: 2019,
    technologies: ['.NET', 'Scraping'],
    type: 'library',
  },
  {
    id: 'gekoproductsscraper',
    name: 'GekoProductsScraper',
    description: 'Product data scraper',
    period: '2019',
    year: 2019,
    technologies: ['.NET', 'Scraping'],
    type: 'tool',
  },
  {
    id: 'ebaynotifier',
    name: 'eBayNotifier',
    description: 'eBay notification tool',
    period: '2018',
    year: 2018,
    technologies: ['.NET'],
    type: 'tool',
  },
  {
    id: 'a2bbay',
    name: 'a2bBay',
    description: 'eBay integration tool',
    period: '2018',
    year: 2018,
    technologies: ['.NET', 'React'],
    type: 'application',
  },
  {
    id: 'evetrillionaire',
    name: 'EveTrillionaire',
    description: 'Eve Online economic simulator',
    period: '2018',
    year: 2018,
    technologies: ['.NET', 'Game'],
    type: 'application',
  },
  {
    id: 'evemoon',
    name: 'EveMoon',
    description: 'Eve Online related project',
    period: '2017',
    year: 2017,
    technologies: ['.NET', 'Game'],
    type: 'application',
  },
  {
    id: 'rayson-logging',
    name: 'Rayson.Logging',
    description: 'Logging infrastructure',
    period: '2017',
    year: 2017,
    technologies: ['.NET', 'Logging'],
    type: 'library',
  },
  {
    id: 'rayson-formulae',
    name: 'Rayson.Formulae',
    description: 'Formula parsing library',
    period: '2017',
    year: 2017,
    technologies: ['.NET', 'Math'],
    type: 'library',
  },
  {
    id: 'rayson-evolution',
    name: 'Rayson.Evolution',
    description: 'Evolutionary computation library',
    period: '2016',
    year: 2016,
    technologies: ['.NET', 'Evolutionary Algorithms'],
    type: 'library',
  },
  {
    id: 'rayson-ai',
    name: 'Rayson.AI',
    description: 'Neural networks library',
    period: '2016',
    year: 2016,
    technologies: ['.NET', 'AI', 'Neural Networks'],
    type: 'library',
  },
  {
    id: 'evolver',
    name: 'Evolver',
    description: 'Evolution framework',
    period: '2015',
    year: 2015,
    technologies: ['.NET', 'Evolutionary Algorithms'],
    type: 'application',
  },
  {
    id: 'rayson-aspects',
    name: 'Rayson.Aspects',
    description: 'AOP aspects library',
    period: '2015',
    year: 2015,
    technologies: ['.NET', 'AOP'],
    type: 'library',
  },
  {
    id: 'rayson-caching',
    name: 'Rayson.Caching',
    description: 'Caching utilities',
    period: '2014',
    year: 2014,
    technologies: ['.NET', 'Caching'],
    type: 'library',
  },
  {
    id: 'rayson-dependencyinjection',
    name: 'Rayson.DependencyInjection',
    description: 'DI container utilities',
    period: '2014',
    year: 2014,
    technologies: ['.NET', 'DI'],
    type: 'library',
  },
  {
    id: 'rayson-data',
    name: 'Rayson.Data',
    description: 'Data utilities library',
    period: '2013',
    year: 2013,
    technologies: ['.NET', 'Data'],
    type: 'library',
  },
  {
    id: 'rayson-wpf',
    name: 'Rayson.WPF',
    description: 'WPF utilities',
    period: '2013',
    year: 2013,
    technologies: ['.NET', 'WPF'],
    type: 'library',
  },
];

export const skills: SkillCategory[] = [
  {
    id: 'languages',
    name: 'Languages',
    skills: ['.NET Core', 'AJAX', 'CSS', 'HTML', 'Java', 'JavaScript', 'JSON', 'Node.js', 'PowerShell', 'Python', 'SASS', 'Scripting', 'Shell Scripting', 'TypeScript', 'VBA', 'XML'],
  },
  {
    id: 'frameworks',
    name: 'Frameworks & Libraries',
    skills: ['ASP.NET Core', 'Bootstrap', 'Entity Framework', 'MVC', 'React', 'React Native', 'Redis', 'Telerik', 'WCF'],
  },
  {
    id: 'cloud-devops',
    name: 'Cloud & DevOps',
    skills: ['Azure', 'CI/CD', 'Cloud architecture', 'Cloud computing', 'Cloud development', 'Cloud infrastructure', 'Docker', 'IIS', 'SaaS', 'S3'],
  },
  {
    id: 'databases',
    name: 'Databases',
    skills: ['Database design', 'Database management', 'Microsoft SQL Server', 'MongoDB', 'MySQL', 'NoSQL', 'PostgreSQL', 'Relational databases', 'SSIS', 'SSRS'],
  },
  {
    id: 'testing-quality',
    name: 'Testing & Quality',
    skills: ['Software testing', 'Test-driven development', 'Unit testing', 'xUnit'],
  },
  {
    id: 'architecture',
    name: 'Architecture & Design',
    skills: ['Application development', 'Design patterns', 'Distributed systems', 'Full-stack development', 'Solution architecture', 'System architecture', 'Systems analysis', 'UML'],
  },
  {
    id: 'methodologies',
    name: 'Methodologies',
    skills: ['Agile', 'Kanban', 'SDLC', 'Waterfall'],
  },
  {
    id: 'apis',
    name: 'APIs & Services',
    skills: ['APIs', 'REST', 'SOAP', 'Web services'],
  },
  {
    id: 'version-control',
    name: 'Version Control & Deployment',
    skills: ['Git', 'GitHub', 'Software deployment', 'VPN'],
  },
  {
    id: 'operating-systems',
    name: 'Operating Systems',
    skills: ['Android', 'Linux', 'Microsoft Windows Server', 'Mobile applications', 'Windows'],
  },
  {
    id: 'soft-skills',
    name: 'Soft Skills',
    skills: ['Analysis skills', 'Business analysis', 'Business requirements', 'Communication skills', 'Continuous improvement', 'Customer service', 'Mentoring', 'Project management', 'Technical support', 'Time management'],
  },
  {
    id: 'additional',
    name: 'Additional',
    skills: ['Back-end development', 'Debugging', 'Deep learning', 'DevOps', 'Elasticsearch', 'Image processing', 'IT', 'Machine learning', 'Microsoft Excel', 'Microsoft Office', 'Power BI', 'Remote access software', 'Scalability', 'System administration', 'Web accessibility'],
  },
];

export const personalDescription = `Professional software developer with experience across diverse sectors including insurance, video streaming, B2B APIs, and material arbitrage. Self-taught programmer since age 15, with deep expertise in the Microsoft/.NET ecosystem and a wide interest in computing and software development. Strong advocate for code quality principles and full development lifecycle experience from requirements gathering through to deployment and support.`;

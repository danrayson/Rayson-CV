export interface WorkExperience {
  id: string;
  company: string;
  period: string;
  title: string;
  description: string;
  bulletPoints: string[];
}

export interface Education {
  id: string;
  period: string;
  qualifications: { subject: string; grade: string }[];
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
  address: '85 Bagots Oak, ST17 9SB',
  phone: '07703 574867',
  email: 'daniel@rayson.dev',
  website: 'www.rayson.dev',
};

export const workExperience: WorkExperience[] = [
  {
    id: 'self-employed',
    company: 'Self Employed (Material Product Arbitrage)',
    period: 'Winter 2023 – Current',
    title: 'N/A',
    description: 'Self-directed research and systems design in material product arbitrage',
    bulletPoints: [
      'Self-directed research and systems design',
      'Responsibility for following regulatory requirements',
      'Automation, data validation, service interfacing',
    ],
  },
  {
    id: 'the-site-doctor',
    company: 'The Site Doctor (Small and Highly Skilled Software House)',
    period: 'Winter 2021 – Winter 2023',
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
    period: 'Winter 2017 – Winter 2021',
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
    period: 'Summer 2014 – Summer 2016',
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
    id: 'dbl ogic',
    company: 'DBLogic (Software House)',
    period: 'Spring 2012 – Spring 2014',
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
    period: '2007 – 2012',
    title: 'Website Development and Support',
    description: 'Freelance website development and support',
    bulletPoints: [
      'Design and implementation of custom websites',
      'General site maintenance and updates',
      'Site specification and general IT consultation',
    ],
  },
];

export const education: Education = {
  id: 'gcse',
  period: '1999-2005',
  qualifications: [
    { subject: 'Mathematics', grade: 'A*' },
  ],
};

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

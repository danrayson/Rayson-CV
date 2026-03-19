import React from 'react';
import { ArrowLeftIcon } from '@heroicons/react/24/outline';
import { Timeline, TimelineCard } from '../components/Timeline';
import { FadeInSection } from '../components/FadeInSection/FadeInSection';
import { useTrackedClick } from '../hooks/useTrackedClick';
import {
  workExperience,
  projects,
  skills,
  personalDetails,
  personalDescription,
  WorkExperience,
  Project,
} from '../data/cv';

function getEndYear(period: string): number {
  if (period.toLowerCase().includes('current') || period.toLowerCase().includes('onwards')) return 2026;
  const match = period.match(/(\d{4})/g);
  if (!match) return 2026;
  return parseInt(match[match.length - 1]);
}

interface RowData {
  year: number;
  work: WorkExperience | null;
  projects: Project[];
}

const TimelinePage: React.FC = () => {
  const sortedWork = [...workExperience].sort((a, b) => getEndYear(a.period) - getEndYear(b.period));
  const sortedProjects = [...projects].sort((a, b) => b.year - a.year);

  const shownYears = new Set<number>();
  const rows: RowData[] = [];

  for (const work of sortedWork) {
    const yearStart = work.yearStart;
    const yearEnd = getEndYear(work.period);

    const overlappingProjects = sortedProjects.filter(p => 
      p.year >= yearStart && p.year <= yearEnd && !shownYears.has(p.year)
    );

    if (overlappingProjects.length > 0) {
      rows.push({
        year: yearEnd,
        work,
        projects: overlappingProjects,
      });
      overlappingProjects.forEach(p => shownYears.add(p.year));
    } else {
      rows.push({
        year: yearEnd,
        work,
        projects: [],
      });
    }
  }

  const remainingProjects = sortedProjects.filter(p => !shownYears.has(p.year));
  for (const project of remainingProjects) {
    rows.push({
      year: project.year,
      work: null,
      projects: [project],
    });
    shownYears.add(project.year);
  }

  rows.sort((a, b) => b.year - a.year);

  return (
    <div className="min-h-screen bg-base-200 text-base-content">
      <button
        data-track data-element-id="timeline-back"
        onClick={useTrackedClick('timeline-back', () => window.history.back())}
        className="fixed top-4 left-4 btn btn-sm btn-ghost z-10"
      >
        <ArrowLeftIcon className="w-5 h-5" />
        Home
      </button>

      <header data-section-id="timeline-hero" className="py-16 text-center px-4">
        <h1 className="text-5xl md:text-7xl font-bold mb-4">Daniel W F Rayson</h1>
        <p className="text-2xl md:text-3xl text-primary font-semibold">Senior DotNet Developer</p>
      </header>

      <main className="container mx-auto px-4 pb-16">
        <FadeInSection delay={100} data-section-id="about-section">
          <section className="mb-16 max-w-4xl mx-auto">
            <h2 className="text-3xl font-bold text-center mb-12">About Me</h2>
            <div className="bg-base-100 p-8 rounded-lg shadow-lg border border-base-300">
              <p className="text-base leading-relaxed">{personalDescription}</p>
            </div>
          </section>
        </FadeInSection>

        <FadeInSection delay={100} data-section-id="timeline-section">
          <section className="mb-16">
            <h2 className="text-3xl font-bold text-center mb-12">Timeline</h2>
          <Timeline>
            <div className="flex flex-col md:flex-row mb-4">
              <div className="w-full md:w-1/2 md:pr-8 text-center font-bold">Work Experience</div>
              <div className="hidden md:block md:w-1/2 md:pl-8 text-center font-bold">Personal Projects</div>
            </div>
            {rows.map((row) => (
              <div key={row.year} className="flex flex-col md:flex-row">
                <div className="w-full md:w-1/2 md:pr-8">
                  {row.work && (
                    <TimelineCard key={row.work.id} size="normal">
                      <div className="bg-base-100 p-1 rounded-lg shadow-lg border border-base-300">
                        <div className="mb-2">
                          <span className="badge badge-primary badge-lg whitespace-nowrap">{row.work.period}</span>
                        </div>
                        <h3 className="text-xl font-bold">{row.work.title}</h3>
                        <p className="text-primary font-medium mb-3">{row.work.company}</p>
                        <p className="text-sm opacity-70 mb-3">{row.work.description}</p>
                        <ul className="list-disc list-inside space-y-1">
                          {row.work.bulletPoints.map((point, idx) => (
                            <li key={idx} className="text-sm">{point}</li>
                          ))}
                        </ul>
                      </div>
                    </TimelineCard>
                  )}
                </div>
                <div className="w-full md:w-1/2 md:pl-8">
                  {row.projects.map((project) => (
                    <TimelineCard key={project.id} size="small">
                      <div className="bg-base-100 p-1 rounded-lg shadow-lg border border-base-300">
                        <div className="flex items-center gap-2">
                          <span className="font-bold text-xs whitespace-nowrap">{project.name}</span>
                          <span className="text-xs opacity-70 truncate">{project.description}</span>
                          <div className="flex gap-1 ml-auto">
                            {project.technologies.slice(0, 2).map((tech) => (
                              <span key={tech} className="badge badge-outline badge-xs whitespace-nowrap">{tech}</span>
                            ))}
                          </div>
                        </div>
                      </div>
                    </TimelineCard>
                  ))}
                </div>
              </div>
            ))}
          </Timeline>
          </section>
        </FadeInSection>

        <FadeInSection delay={100} data-section-id="skills-section">
          <section className="mb-16 max-w-4xl mx-auto">
            <h2 className="text-3xl font-bold text-center mb-12">Skills</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {skills.map((category) => (
                <FadeInSection key={category.id} delay={100}>
                  <div className="bg-base-100 p-6 rounded-lg shadow-lg border border-base-300">
                    <h3 className="text-xl font-bold text-primary mb-4">{category.name}</h3>
                    <div className="flex flex-wrap gap-2">
                      {category.skills.map((skill) => (
                        <span key={skill} className="badge badge-outline badge-lg">{skill}</span>
                      ))}
                    </div>
                  </div>
                </FadeInSection>
              ))}
            </div>
          </section>
        </FadeInSection>

        <FadeInSection delay={100} data-section-id="contact-section">
          <section className="max-w-4xl mx-auto">
          <h2 className="text-3xl font-bold text-center mb-12">Contact</h2>
          <div className="bg-base-100 p-8 rounded-lg shadow-lg border border-base-300">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="flex items-center gap-3">
                <span className="text-primary">
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                  </svg>
                </span>
                <span>{personalDetails.address}</span>
              </div>
              <div className="flex items-center gap-3">
                <span className="text-primary">
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                  </svg>
                </span>
                <span>{personalDetails.phone}</span>
              </div>
              <div className="flex items-center gap-3">
                <span className="text-primary">
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                  </svg>
                </span>
                <span>{personalDetails.email}</span>
              </div>
              <div className="flex items-center gap-3">
                <span className="text-primary">
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 12a9 9 0 01-9 9m9-9a9 9 0 00-9-9m9 9H3m9 9a9 9 0 01-9-9m9 9c1.657 0 3-4.03 3-9s-1.343-9-3-9m0 18c-1.657 0-3-4.03-3-9s1.343-9 3-9m-9 9a9 9 0 019-9" />
                  </svg>
                </span>
                <a href={`https://${personalDetails.website}`} target="_blank" rel="noopener noreferrer" className="link link-primary">
                  {personalDetails.website}
                </a>
              </div>
            </div>
          </div>
        </section>
        </FadeInSection>
      </main>
    </div>
  );
};

export default TimelinePage;

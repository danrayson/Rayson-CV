import React from 'react';
import { ArrowLeftIcon } from '@heroicons/react/24/outline';
import { Timeline, TimelineCard } from '../components/Timeline';
import {
  workExperience,
  education,
  skills,
  personalDetails,
  personalDescription,
} from '../data/cv';

const TimelinePage: React.FC = () => {
  return (
    <div className="min-h-screen bg-base-200 text-base-content">
      <button
        onClick={() => window.history.back()}
        className="fixed top-4 left-4 btn btn-sm btn-ghost z-10"
      >
        <ArrowLeftIcon className="w-5 h-5" />
        Home
      </button>

      <header className="py-16 text-center px-4">
        <h1 className="text-5xl md:text-7xl font-bold mb-4">Daniel W F Rayson</h1>
        <p className="text-2xl md:text-3xl text-primary font-semibold">Senior DotNet Developer</p>
      </header>

      <main className="container mx-auto px-4 pb-16">
        <section className="mb-16">
          <h2 className="text-3xl font-bold text-center mb-12">Work Experience</h2>
          <Timeline>
            {workExperience.map((job) => (
              <TimelineCard key={job.id}>
                <div className="timeline-node absolute left-4 md:left-1/2 w-4 h-4 rounded-full bg-primary transform -translate-x-1/2 z-10 ring-4 ring-base-200" />
                <div className="bg-base-100 p-6 rounded-lg shadow-lg border border-base-300">
                  <div className="mb-2">
                    <span className="badge badge-primary badge-lg">{job.period}</span>
                  </div>
                  <h3 className="text-xl font-bold">{job.title}</h3>
                  <p className="text-primary font-medium mb-3">{job.company}</p>
                  <p className="text-sm opacity-70 mb-3">{job.description}</p>
                  <ul className="list-disc list-inside space-y-1">
                    {job.bulletPoints.map((point, idx) => (
                      <li key={idx} className="text-sm">{point}</li>
                    ))}
                  </ul>
                </div>
              </TimelineCard>
            ))}
          </Timeline>
        </section>

        <section className="mb-16 max-w-4xl mx-auto">
          <h2 className="text-3xl font-bold text-center mb-12">Education</h2>
          <div className="bg-base-100 p-8 rounded-lg shadow-lg border border-base-300">
            <div className="text-center mb-6">
              <span className="badge badge-primary badge-lg">{education.period}</span>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {education.qualifications.map((qual) => (
                <div key={qual.subject} className="flex justify-between items-center p-4 bg-base-200 rounded">
                  <span className="font-medium">{qual.subject}</span>
                  <span className="badge badge-secondary badge-lg">{qual.grade}</span>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="mb-16 max-w-4xl mx-auto">
          <h2 className="text-3xl font-bold text-center mb-12">Skills</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {skills.map((category) => (
              <div key={category.id} className="bg-base-100 p-6 rounded-lg shadow-lg border border-base-300">
                <h3 className="text-xl font-bold text-primary mb-4">{category.name}</h3>
                <div className="flex flex-wrap gap-2">
                  {category.skills.map((skill) => (
                    <span key={skill} className="badge badge-outline badge-lg">{skill}</span>
                  ))}
                </div>
              </div>
            ))}
          </div>
        </section>

        <section className="mb-16 max-w-4xl mx-auto">
          <h2 className="text-3xl font-bold text-center mb-12">About Me</h2>
          <div className="bg-base-100 p-8 rounded-lg shadow-lg border border-base-300">
            <p className="text-lg leading-relaxed">{personalDescription}</p>
          </div>
        </section>

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
      </main>
    </div>
  );
};

export default TimelinePage;

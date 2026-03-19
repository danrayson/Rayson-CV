import { useState } from 'react';
import { ArrowLeftIcon, PhoneIcon } from '@heroicons/react/24/outline';
import { contactService } from '../services/contactService';
import { useFormErrors } from '../hooks/useFormErrors';
import { useTrackedClick } from '../hooks/useTrackedClick';
import ValidationMessages from '../components/ui/ValidationMessages';

const ContactPage: React.FC = () => {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    subject: '',
    message: ''
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitStatus, setSubmitStatus] = useState<'idle' | 'success'>('idle');
  const { errors, showErrors, handleApiError, clearErrors } = useFormErrors();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setFormData(prev => ({
      ...prev,
      [e.target.name]: e.target.value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    clearErrors();

    try {
      await contactService.sendContactEmail(formData);
      setSubmitStatus('success');
      setFormData({ name: '', email: '', subject: '', message: '' });
    } catch (error) {
      handleApiError(error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-base-200 text-base-content p-4 flex flex-col items-center justify-center">
      <button
        data-track data-element-id="contact-back"
        onClick={useTrackedClick('contact-back', () => window.history.back())}
        className="fixed top-4 left-4 btn btn-sm btn-ghost z-10"
      >
        <ArrowLeftIcon className="w-5 h-5" />
        Home
      </button>

      <div className="max-w-2xl w-full">
        <h1 className="text-4xl font-bold mb-8 text-center">Contact Me</h1>

        <div className="card bg-base-100 shadow-xl mb-8">
          <div className="card-body">
            <div className="flex items-center justify-center gap-0.5">
              <PhoneIcon className="w-5 h-5" />
              <a href="tel:+447703574867" className="text-lg font-semibold hover:underline">
                07703 574867
              </a>
            </div>
          </div>
        </div>

        <form className="card bg-base-100 shadow-xl">
          <div className="card-body">
            <h2 className="card-title mb-4">Send a Message</h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="form-control">
                <label className="label">
                  <span className="label-text">Name</span>
                </label>
                <input
                  type="text"
                  name="name"
                  value={formData.name}
                  onChange={handleChange}
                  className="input input-bordered"
                  placeholder="Your name"
                />
              </div>

              <div className="form-control">
                <label className="label">
                  <span className="label-text">Email</span>
                </label>
                <input
                  type="text"
                  name="email"
                  value={formData.email}
                  onChange={handleChange}
                  className="input input-bordered"
                  placeholder="your@email.com"
                />
              </div>
            </div>

            <div className="form-control">
              <label className="label">
                <span className="label-text">Subject</span>
              </label>
              <input
                type="text"
                name="subject"
                value={formData.subject}
                onChange={handleChange}
                className="input input-bordered"
                placeholder="What's this about?"
              />
            </div>

            <div className="form-control">
              <label className="label">
                <span className="label-text">Message</span>
              </label>
              <textarea
                name="message"
                value={formData.message}
                onChange={handleChange}
                className="textarea textarea-bordered h-32"
                placeholder="Your message..."
              />
            </div>

            <ValidationMessages showErrors={showErrors} errors={errors} />

            {submitStatus === 'success' && (
              <div className="alert alert-success mb-4">
                <span>Message sent successfully! I'll get back to you soon.</span>
              </div>
            )}

            <div className="form-control mt-4">
              <button
                type="submit"
                data-track data-element-id="send-message"
                onClick={useTrackedClick('send-message', () => handleSubmit({} as unknown as React.FormEvent))}
                disabled={isSubmitting}
                className="btn btn-primary"
              >
                {isSubmitting ? (
                  <span className="loading loading-spinner"></span>
                ) : (
                  'Send Message'
                )}
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ContactPage;

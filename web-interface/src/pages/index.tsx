import React, { useState, useEffect } from 'react';
import Head from 'next/head';
import Link from 'next/link';
import Image from 'next/image';
import { motion } from 'framer-motion';
import { FaDiscord, FaTwitter, FaYoutube, FaPlayCircle, FaInfoCircle, FaUserPlus, FaSignInAlt } from 'react-icons/fa';

// Component for the animated hero section
const HeroSection = () => {
  return (
    <div className="relative h-screen flex items-center justify-center overflow-hidden">
      {/* Background video or image would go here in production */}
      <div className="absolute inset-0 bg-gradient-to-r from-blue-900/80 to-purple-900/80 z-10"></div>
      
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 z-20 text-center">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8 }}
          className="text-center"
        >
          <h1 className="text-5xl md:text-7xl font-bold text-white mb-6">
            <span className="text-blue-400">Vibe</span>Mod
          </h1>
          <p className="text-xl md:text-2xl text-gray-200 max-w-3xl mx-auto mb-8">
            A hyper-realistic MMORPG inspired by Garry's Mod, featuring Hide-n-Seek and Infection 
            game modes on Call of Duty inspired maps.
          </p>
          <div className="flex flex-col sm:flex-row justify-center gap-4">
            <motion.button
              whileHover={{ scale: 1.05 }}
              whileTap={{ scale: 0.95 }}
              className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-6 rounded-lg flex items-center justify-center gap-2"
            >
              <FaPlayCircle className="text-xl" />
              Play Now
            </motion.button>
            <motion.button
              whileHover={{ scale: 1.05 }}
              whileTap={{ scale: 0.95 }}
              className="bg-gray-700 hover:bg-gray-800 text-white font-bold py-3 px-6 rounded-lg flex items-center justify-center gap-2"
            >
              <FaInfoCircle className="text-xl" />
              Learn More
            </motion.button>
          </div>
        </motion.div>
      </div>
      
      {/* Wave divider */}
      <div className="absolute bottom-0 left-0 right-0 z-10">
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 320">
          <path
            fill="#111827"
            fillOpacity="1"
            d="M0,128L48,138.7C96,149,192,171,288,170.7C384,171,480,149,576,160C672,171,768,213,864,213.3C960,213,1056,171,1152,144C1248,117,1344,107,1392,101.3L1440,96L1440,320L1392,320C1344,320,1248,320,1152,320C1056,320,960,320,864,320C768,320,672,320,576,320C480,320,384,320,288,320C192,320,96,320,48,320L0,320Z"
          ></path>
        </svg>
      </div>
    </div>
  );
};

// Component for features section
const FeaturesSection = () => {
  const features = [
    {
      title: "Ultra-Realistic Graphics",
      description: "Experience gaming like never before with our HDRP powered visuals and next-gen lighting system.",
      icon: "🎮"
    },
    {
      title: "Multiple Game Modes",
      description: "Hide and Seek, Infection, and Sandbox modes provide endless hours of gameplay variety.",
      icon: "🎯"
    },
    {
      title: "Physics-Based Gameplay",
      description: "Interact with everything in the world using our advanced physics system inspired by Garry's Mod.",
      icon: "🧪"
    },
    {
      title: "Call of Duty Inspired Maps",
      description: "Battle across iconic locations reimagined with improved detail and interactivity.",
      icon: "🗺️"
    },
    {
      title: "Free To Play",
      description: "Jump in and play with friends without spending a dime. Optional cosmetics available to support development.",
      icon: "💰"
    },
    {
      title: "Cross-Platform",
      description: "Play in your browser or download the client for maximum performance.",
      icon: "🖥️"
    }
  ];
  
  return (
    <div className="bg-gray-900 py-20">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="text-center mb-16">
          <h2 className="text-4xl font-bold text-white mb-4">Game Features</h2>
          <p className="text-xl text-gray-400 max-w-3xl mx-auto">
            VibeMod combines the best elements of sandbox games with fast-paced multiplayer action.
          </p>
        </div>
        
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          {features.map((feature, index) => (
            <motion.div
              key={index}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ duration: 0.5, delay: index * 0.1 }}
              className="bg-gray-800 rounded-xl p-8 hover:bg-gray-700 transition-colors"
            >
              <div className="text-4xl mb-4">{feature.icon}</div>
              <h3 className="text-xl font-bold text-white mb-2">{feature.title}</h3>
              <p className="text-gray-400">{feature.description}</p>
            </motion.div>
          ))}
        </div>
      </div>
    </div>
  );
};

// Component for game modes section
const GameModesSection = () => {
  const [activeMode, setActiveMode] = useState(0);
  
  const gameModes = [
    {
      name: "Hide and Seek",
      description: "One player is designated as the seeker, while others hide throughout the map. The seeker must find all hiding players before time runs out.",
      image: "/images/hide-and-seek.jpg" // Placeholder - would need an actual image in production
    },
    {
      name: "Infection",
      description: "Start with one infected player who must spread the infection to others. Survivors must evade the growing horde until time runs out.",
      image: "/images/infection.jpg" // Placeholder
    },
    {
      name: "Sandbox",
      description: "Freely explore and interact with the world. Build contraptions, experiment with physics, and create your own games within the game.",
      image: "/images/sandbox.jpg" // Placeholder
    }
  ];
  
  return (
    <div className="bg-gray-800 py-20">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="text-center mb-12">
          <h2 className="text-4xl font-bold text-white mb-4">Game Modes</h2>
          <p className="text-xl text-gray-400 max-w-3xl mx-auto">
            Multiple ways to play, one incredible experience.
          </p>
        </div>
        
        <div className="flex flex-col lg:flex-row gap-8">
          <div className="lg:w-1/3">
            <div className="bg-gray-900 rounded-xl p-4">
              {gameModes.map((mode, index) => (
                <button
                  key={index}
                  onClick={() => setActiveMode(index)}
                  className={`w-full text-left p-4 mb-2 rounded-lg transition-colors ${
                    activeMode === index 
                      ? 'bg-blue-600 text-white' 
                      : 'bg-gray-800 text-gray-300 hover:bg-gray-700'
                  }`}
                >
                  <h3 className="text-xl font-bold">{mode.name}</h3>
                </button>
              ))}
            </div>
          </div>
          
          <motion.div 
            key={activeMode}
            initial={{ opacity: 0, x: 20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.5 }}
            className="lg:w-2/3 bg-gray-900 rounded-xl overflow-hidden"
          >
            <div className="aspect-w-16 aspect-h-9 relative h-64 lg:h-80">
              <div className="absolute inset-0 bg-gradient-to-t from-gray-900 to-transparent z-10"></div>
              {/* Placeholder for actual images */}
              <div className="absolute inset-0 bg-blue-900/30"></div>
            </div>
            <div className="p-6">
              <h3 className="text-2xl font-bold text-white mb-2">{gameModes[activeMode].name}</h3>
              <p className="text-gray-400">{gameModes[activeMode].description}</p>
            </div>
          </motion.div>
        </div>
      </div>
    </div>
  );
};

// Component for newsletter signup
const NewsletterSection = () => {
  const [email, setEmail] = useState('');
  
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Handle newsletter signup logic here
    alert(`Thank you! ${email} has been registered for updates.`);
    setEmail('');
  };
  
  return (
    <div className="bg-blue-900 py-16">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex flex-col lg:flex-row items-center justify-between gap-8">
          <div className="lg:w-1/2">
            <h2 className="text-3xl font-bold text-white mb-4">Stay Updated</h2>
            <p className="text-blue-200 mb-6">
              Subscribe to our newsletter for development updates, early access opportunities, and special events.
            </p>
          </div>
          
          <div className="lg:w-1/2">
            <form onSubmit={handleSubmit} className="flex flex-col sm:flex-row gap-2">
              <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Enter your email"
                required
                className="flex-grow px-4 py-3 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <motion.button
                whileHover={{ scale: 1.05 }}
                whileTap={{ scale: 0.95 }}
                type="submit"
                className="bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-6 rounded-lg"
              >
                Subscribe
              </motion.button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

// Main component for the homepage
export default function Home() {
  return (
    <div className="min-h-screen bg-gray-900 text-white">
      <Head>
        <title>VibeMod - Ultra-Realistic MMORPG</title>
        <meta name="description" content="A hyper-realistic MMORPG inspired by Garry's Mod with hide and seek & infection game modes on Call of Duty inspired maps." />
        <link rel="icon" href="/favicon.ico" />
      </Head>
      
      <header className="absolute top-0 left-0 right-0 z-30">
        <nav className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center">
              {/* Logo placeholder */}
              <div className="font-bold text-2xl text-white">
                <span className="text-blue-400">Vibe</span>Mod
              </div>
            </div>
            
            <div className="hidden md:flex items-center space-x-8">
              <Link href="/about" className="text-gray-300 hover:text-white transition-colors">
                About
              </Link>
              <Link href="/features" className="text-gray-300 hover:text-white transition-colors">
                Features
              </Link>
              <Link href="/community" className="text-gray-300 hover:text-white transition-colors">
                Community
              </Link>
              <Link href="/support" className="text-gray-300 hover:text-white transition-colors">
                Support
              </Link>
            </div>
            
            <div className="flex items-center space-x-4">
              <Link href="/login" className="text-gray-300 hover:text-white transition-colors flex items-center">
                <FaSignInAlt className="mr-1" /> Login
              </Link>
              <Link href="/register" className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg transition-colors flex items-center">
                <FaUserPlus className="mr-1" /> Register
              </Link>
            </div>
          </div>
        </nav>
      </header>
      
      <main>
        <HeroSection />
        <FeaturesSection />
        <GameModesSection />
        <NewsletterSection />
      </main>
      
      <footer className="bg-gray-900 text-white py-12">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
            <div>
              <h3 className="text-xl font-bold mb-4">VibeMod</h3>
              <p className="text-gray-400">
                A hyper-realistic MMORPG inspired by Garry's Mod with unique gameplay elements.
              </p>
            </div>
            
            <div>
              <h3 className="text-lg font-bold mb-4">Links</h3>
              <ul className="space-y-2">
                <li><Link href="/about" className="text-gray-400 hover:text-white transition-colors">About</Link></li>
                <li><Link href="/features" className="text-gray-400 hover:text-white transition-colors">Features</Link></li>
                <li><Link href="/community" className="text-gray-400 hover:text-white transition-colors">Community</Link></li>
                <li><Link href="/support" className="text-gray-400 hover:text-white transition-colors">Support</Link></li>
              </ul>
            </div>
            
            <div>
              <h3 className="text-lg font-bold mb-4">Legal</h3>
              <ul className="space-y-2">
                <li><Link href="/terms" className="text-gray-400 hover:text-white transition-colors">Terms of Service</Link></li>
                <li><Link href="/privacy" className="text-gray-400 hover:text-white transition-colors">Privacy Policy</Link></li>
                <li><Link href="/cookies" className="text-gray-400 hover:text-white transition-colors">Cookie Policy</Link></li>
              </ul>
            </div>
            
            <div>
              <h3 className="text-lg font-bold mb-4">Follow Us</h3>
              <div className="flex space-x-4">
                <a href="https://discord.gg/vibemod" target="_blank" rel="noopener noreferrer" className="text-gray-400 hover:text-white transition-colors">
                  <FaDiscord className="text-2xl" />
                </a>
                <a href="https://twitter.com/vibemod" target="_blank" rel="noopener noreferrer" className="text-gray-400 hover:text-white transition-colors">
                  <FaTwitter className="text-2xl" />
                </a>
                <a href="https://youtube.com/vibemod" target="_blank" rel="noopener noreferrer" className="text-gray-400 hover:text-white transition-colors">
                  <FaYoutube className="text-2xl" />
                </a>
              </div>
            </div>
          </div>
          
          <div className="border-t border-gray-800 mt-8 pt-8 text-center text-gray-500">
            <p>&copy; {new Date().getFullYear()} VibeMod. All rights reserved.</p>
          </div>
        </div>
      </footer>
    </div>
  );
} 
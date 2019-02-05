import React from 'react'
import { NavLink } from 'react-router-dom'

const sections = [
  { link: '/content', title: 'Content' },
  { link: '/media', title: 'Media' },
  { link: '/analytics', title: 'Analytics' },
  { link: '/security', title: 'Security' },
  { link: '/system', title: 'System' }
]

export const Header = () => (
  <header className='navbar'>
    <div className='container'>
      <div className='navbar-start'>
        <NavLink className='navbar-item' to='/'><strong>KASBAH</strong></NavLink>
      </div>

      <div className='navbar-end'>
        {sections.map((ent, index) => (
          <NavLink key={index} className='navbar-item' activeClassName='is-active' to={ent.link}>{ent.title}</NavLink>
        ))}
        <NavLink className='navbar-item' to='/login'>Logout</NavLink>
      </div>
    </div>
  </header>
)

export default Header

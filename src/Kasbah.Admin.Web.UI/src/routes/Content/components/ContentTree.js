import React from 'react'
import { Link } from 'react-router'

export const View = () => (
  <ul className='content-tree'>
    <li>
      <Link to='/content/1'>Website</Link>
      <ul>
        <li>
          <Link to='/content/1'>Home</Link>
          <ul>
            <li>
              <Link to='/content/1'>Contact</Link>
              <ul>
                <li>...</li>
              </ul>
            </li>
            <li>
              <Link to='/content/1'>About</Link>
              <ul>
                <li>...</li>
              </ul>
            </li>
          </ul>
        </li>
      </ul>
    </li>
  </ul>
)

export default View

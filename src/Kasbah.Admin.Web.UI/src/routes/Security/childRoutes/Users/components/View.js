import React from 'react'

export const View = () => (
  <div>
    <table className='table is-hover'>
      <thead>
        <tr>
          <th>Name</th>
          <th style={{ width: 200 }}>Username</th>
          <th style={{ width: 200 }}>Email</th>
          <th style={{ width: 200 }}>Last login</th>
          <th />
        </tr>
      </thead>
    </table>
  </div>
)

export default View

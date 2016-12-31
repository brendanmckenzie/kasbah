import React from 'react'
import Controls from './Controls'

export const FieldEditor = () => (
  <div className='control'>
    <label className='label'>Username</label>
    <Controls.TextField />
  </div>
)

export default FieldEditor

import React from 'react'

const Date = ({ input: { value, onChange }, className }) => (
  <input type='date' className={className} onChange={onChange} value={value} />
)

Date.propTypes = {
  input: React.PropTypes.object.isRequired,
  className: React.PropTypes.string
}

Date.alias = 'date'

export default Date

import React from 'react'
import PropTypes from 'prop-types'

const Date = ({ input: { value, onChange }, className }) => (
  <input type='date' className={className} onChange={onChange} value={value} />
)

Date.propTypes = {
  input: PropTypes.object.isRequired,
  className: PropTypes.string
}

Date.alias = 'date'

export default Date

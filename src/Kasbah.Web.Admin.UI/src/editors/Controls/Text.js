import React from 'react'

const Text = ({ input: { value, onChange }, className }) => (
  <input type='text' className={className} onChange={onChange} value={value} />
)

Text.propTypes = {
  input: React.PropTypes.object.isRequired,
  className: React.PropTypes.string
}

Text.alias = 'text'

export default Text

import React from 'react'
import PropTypes from 'prop-types'

const Bias = ({ input: { value, onChange }, className }) => (
  <p>I'm the bias editor.  I'll help you indicate what this piece of content is bias towards. <small>when I'm eventually built...</small></p>
)

Bias.propTypes = {
  input: PropTypes.object.isRequired,
  className: PropTypes.string
}

Bias.alias = 'bias'

export default Bias

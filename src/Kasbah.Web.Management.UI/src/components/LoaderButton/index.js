import React from 'react'
import PropTypes from 'prop-types'

const LoaderButton = (props) => {
  const attrs = {
    onClick: props.onClick,
    style: props.style,
    className: [
      props.className,
      'button',
      props.task.loading ? 'is-loading' : null,
      props.medium ? 'is-medium' : null,
      props.small ? 'is-small' : null,
      props.primary ? 'is-primary' : null,
      props.warning ? 'is-warning' : null].join(' ').trim()
  }
  return (
    <button {...attrs}>{props.children}</button>
  )
}

LoaderButton.propTypes = {
  onClick: PropTypes.func.isRequired,
  className: PropTypes.string,
  style: PropTypes.string,
  task: PropTypes.object,
  medium: PropTypes.bool,
  small: PropTypes.bool,
  primary: PropTypes.bool,
  warning: PropTypes.bool,
  children: PropTypes.node
}

export default LoaderButton

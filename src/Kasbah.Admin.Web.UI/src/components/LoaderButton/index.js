import React from 'react'

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
  onClick: React.PropTypes.func.isRequired,
  className: React.PropTypes.string,
  style: React.PropTypes.string,
  task: React.PropTypes.object,
  medium: React.PropTypes.bool,
  small: React.PropTypes.bool,
  primary: React.PropTypes.bool,
  warning: React.PropTypes.bool,
  children: React.PropTypes.node
}

export default LoaderButton

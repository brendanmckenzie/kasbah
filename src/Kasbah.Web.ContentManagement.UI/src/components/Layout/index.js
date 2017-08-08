import React from 'react'
import PropTypes from 'prop-types'

export const Column = ({ children, className }) =>
  <div className={'column ' + (className || '')}>{children}</div>

Column.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string
}

export const Columns = ({ children, className }) =>
  <div className={'columns ' + (className || '')}>{children}</div>

Columns.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string
}

export const Container = ({ children, className }) =>
  <div className={'container ' + (className || '')}>{children}</div>

Container.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string
}

export const Section = ({ children, className }) =>
  <div className={'section ' + (className || '')}>{children}</div>

Section.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string
}

export const Field = ({ children, className }) =>
  <div className={'field ' + (className || '')}>{children}</div>

Field.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string
}

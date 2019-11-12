import React from 'react';
import ShallowRenderer from 'react-test-renderer/shallow';
import ContentPreview from '../index';

describe('ContentPreview', () => {
    it('render correctly', () => {
        let props = {
            text: '**bold**'
        };

        let result = (new ShallowRenderer()).render(<ContentPreview {...props} />);

        expect(result).toMatchSnapshot();
    });
});